---
marp: true
theme: default
paginate: true
---

# Approval тестирование в .net

---

## Константин Финагин

**Старший разработчик в Лаборатории Касперского (с 2023)**   
Ранее — старший разработчик в DataArt (с 2014)  

📧 konstantin.finagin@gmail.com

---

## О чём пойдёт речь?

- Юнит тесты, рост их сложности 📈
- Как с этим бороться с помощью апрувал тестов ✅
- Чем поможет форматирование данных в виде таблиц 📊
- Уровень джуниор+ и выше 🚀

---

## Зачем нужны тесты?

- Тесты помогают снижать **технический долг** и гарантируют стабильность
- Тесно связаны с **Dependency Injection**
- Со временем **моки и ассерты** становятся частью процесса разработки

--- 

## Когда пишутся тесты?

- Есть логика, которую мы хотим протестировать (рефакторинги, легаси, сначала придумали логику)
- Еще нет логики, которую мы хотим протестировать, и сначала пишем тест (TDD)
- (Казалось бы) протестированная логика упала на проде, и мы хотим её воспроизвести и зафиксировать в тесте (реальная жизнь) <!-- _fragment_ -->

---

## Структура теста из трех секций

```csharp
[Fact]
public void Test()
{
    // Arrange
    _service1Mock.When(...).Do(...);
    _service2Mock.When(...).Returns(...);

    // Act
    var outputData = _testedService.DoSomething(inputData);
        // внутри использует _service1Mock, _service2Mock
    
    // Assert
    Assert.Equal("ExpectedValue", outputData.Field1);
    _service1.Received(1).DoSomethingElse();
}

```

---

## Когда тесты начинают усложняться?

- Простые тесты обычно не вызывают проблем
- Если тест сложно писать, возможно, нужно **выделить отдельные операции в другой сервис**
- Иногда это невозможно, и сложность теста растет вместе с логикой метода
- **Act** остается коротким, а **Arrange** и **Assert** могут становиться всё больше <!-- _fragment_ -->

Об этом сегодня и пойдет речь!

---

# Часть 1. Рост сложности секции Assert и необходимость Approvals

---

## Сценарий 1. Много ассертов и мы знакомимся с Approval тестами


- Метод возвращает сложный объект
- Секция Assert растет с ростом количества пропертей объекта

--- 

```csharp
[Fact]
public void Test()
{
    // Arrange
    var dataReceivedByMock1 = new Data1();
    _service1Mock.When(...).Do(_ => dataReceivedByMock1 = ...);
    _service2Mock.When(...).Return();

    // Act
    var outputData = _testedService.DoSomething(inputData);
    
    // Assert
    Assert.Equal("ExpectedValue1", outputData.Field1);
    Assert.Equal("ExpectedValue2", outputData.Field2);

    Assert.Equal("ExpectedValue51", outputData.Field5.Child1);
    Assert.Equal("ExpectedValue52", outputData.Field6.Child2);

    Assert.Equal("DataReceivedByMock1", dataReceivedByMock1.Field1);
    Assert.Equal("DataReceivedByMock2", dataReceivedByMock1.Field2);
}
```

---

Это может быть глубоко вложенный иерархический объект с десятками полей с числовыми значениями и категориями, которые для проверки корректности нужно оценивать полностью.

## Реальный пример - иерархии позиций и финансовых тразнакций
---

```csharp
public class BasePositionGetResponse
{
    public string CurrencyCode { get; set; }
    public decimal? Funded { get; set; }
    public decimal? Unfunded { get; set; }
    public decimal? Committed { get; set; }
    public decimal? CashOnCash { get; set; }
    public decimal? FundedPIK { get; set; }
    //...
}

public class ExtendedPositionGetResponse : BasePositionGetResponse
{
    public string DimensionId { get; set; }
    public string DimensionName { get; set; }
    public List<ExtendedPositionGetResponse> ChildPositionsResponses { get; set; }
    public Dictionary<int, BasePositionGetResponse> UnitrancheClassPositions { get; set; }
    //...
}
```

```csharp
await _positionService.GetPositionsAsync(request); // один вызов! нужен тест
```

---

## Сложности

- Сколько ассертов?
- Проверка логгирования и сайд эффектов?
- Пропустили ассерт одного из полей?
- Объект изменился и добавилось новое поле?
- 100% "покрытие" - может не отражать реальное поведение
- Визуальный мусор 

---

## Как проверить **все** данные без ада ассертов?

- Сериализовать в JSON и сравнить строку

Да, но
- Поймаем только первое расхождение
- Должны заранее знать результат

Поэтому
- Approval-тестирование!

---

## Approval-тест

- автоматизация сравнения строк, которые мы получили в результате выполнения теста
- 1 прогон - .received-файл (тест красный)
- Diff Tool - ручной аппрув (одобрение результата)
- 2 прогон - .approved-файл (тест зеленый)
- Логика изменилась - .received не совпадает - тест снова красный, проверяем изменения в Diff Tool

Инструменты:
```
https://github.com/approvals/ApprovalTests.Net
```

```
> dotnet add package ApprovalTests
```
---

```csharp 
[UseReporter(typeof(DiffReporter))]
[UseApprovalSubdirectory("Results")]
public class ApprovalTest
{
    [Fact]
    public void Test()
    {
        // Arrange
        ...
        
        // Act
        var outputData = _testedService.DoSomething(inputData);
        
        // Assert
        var serialized = JsonSerializer.Serialize(outputData, FormattedOptions);
        
        // Approval test! 
        Approvals.Verify(serialized);
    }
}
```

---

## Demo time!

---

## CI/CD

- Diff tool запускаем только локальной
- Для CI/CD пайплайна - QuietReporter

```csharp

if (IsCiCdEnvironment())
{
	Approvals.SetReporter(new QuietReporter());
}
else
{
	Approvals.SetReporter(new DiffReporter());
}
```

---

- Можно сделать это и атрибутами для compile-time, если локальные тесты всегда прогоняются в дебаге, а удаленные - в релизе.
- Можно указать директорию для результатов тестов

```csharp
#if DEBUG
	[UseReporter(typeof(DiffReporter))]
#else
	[UseReported(typeof(QuietReporter))]
#endif
	[UseApprovalSubdirectory("Results")]
public class ApprovalTest
{
	...
}
```

---

## Частичное сравнение и динамические данные



---
- Не нужна вся информация объекта (только подмножество) - используем анонимные объекты
```csharp
...
var subset = new
{
    largeObject.Id,
    largeObject.Name,
    CreatedAt = largeObject.CreatedAt.ToString("O")
};

string json = JsonSerializer.Serialize(subset, new JsonSerializerOptions
{
    WriteIndented = true
});

Approvals.Verify(json);
```

---
- Динамические данные - Guid, DateTime - используем DI моки/врапперы

```csharp
public static class DateTimeWrapper
{
    private static DateTime? _fixedTime;
    public static DateTime UtcNow => _fixedTime ?? DateTime.UtcNow;
    public static void Set(DateTime fixedTime) => _fixedTime = fixedTime;
    public static void Reset() => _fixedTime = null;
}
```

```csharp
//replace DateTime.UtcNow()
var date = DateTimeWrapper.UtcNow();
```
```csharp
public ApprovalTest()
{
    DateTimeWrapper.Set(DateTime.Parse("2025-01-01"));
}
```

---

- Непредсказуемые результаты, где для тестирования которых нужно: 
    - искать какое-то значение в динамическом списке
    - сравнивать по Contains или StartsWith, итд

- Нет серебряной пули!
    - бороться со случайностью в коде, 
    - приспосабливать для выборки детерминированных данных , 
    - писать кастомные юнит тесты уже без использования аппрувалов

---

## Сценарий 2. На выходе - табличные данные.

- Иногда результатом работы тестируемого метода могут быть табличные данные
    - идут в другой сервис или базу
    - DataTable для массовой bulk-вставки в базу, или 
    - csv с отчетами. 
- Могут и не быть табличными, может быть стандартный json, описывающий иерархию объектов. 
    - но в итоге превращаться в записи в БД. 

- В этом случае Json не даст легкой для восприятия информации

---

```csharp
[Test]
public class ApprovalTest
{
    // Arrange
    DataTable savedData = null;
    _repositoryMock
        .When(r => r.SaveDataTable(Arg.Is<DataTable>()))
        .Do(saved => savedData = saved);
    ...

    // Act
    _service.DoSomethingAndSave();
	
    // Assert
    var json = JsonHelper.ToJson(savedData);
    Approvals.Approve(json); // ???

    // сохранение в Json не даст визуально полезной информации
}
```

---

Можно переписать это как

```csharp
[Test]
public class ApprovalTest
{
	// Arrange
    DataTable savedData = null;
    _repositoryMock
        .When(r => r.SaveDataTable(Arg.Is<DataTable>()))
        .Do(saved => savedData = saved);
    ...

    // Act
    _service.DoSomethingAndSave();
	
    // Assert
    var table = TableFormatterHelper.ToAsciiTable(savedData);
    Approvals.Approve(table);
}
```

---

Хелпер форматирует объект или dataTable в виде ascii таблицы

```
 |            | Committed  | Funded     | Funded PIK | Unfunded   | 
 |----------------------------------------------------------------| 
 |            | 9700000.00 | 2700000.00 | 21917.80   | 6978082.20 | 

 | CustomerId | Committed  | Funded     | Funded PIK | Unfunded   | 
 |----------------------------------------------------------------| 
 | 2          | 6790000.00 | 1890000.00 | 15342.47   | 4884657.53 | 
 | 3          | 2910000.00 | 810000.00  | 6575.34    | 2093424.66 | 
```

---

Или вот так

```
 | Level       | Customer | ParticipationId | Expected | Actual | 
 |--------------------------------------------------------------| 
 | Lender      | 2        | null            | 16.66    | 16.66  | 
 | Grantor     | 2        | 1011            | 6.25     | 6.25   | 
 | Participant | 11       | 1011            | 3.12     | 3.12   | 
 | Participant | 12       | 1011            | 1.88     | 1.88   | 
 | Participant | 13       | 1011            | 1.25     | 1.25   | 
 | Lender      | 3        | null            | 16.67    | 16.67  | 
 | Lender      | 4        | null            | 16.67    | 16.67  | 
```

- при таком форматировании удобно видеть значения полей при аппруве
- при красном тесте и запуске diff можно увидеть конкретное расположение поля, которое изменилось или не совпадало с ожидаемым.

---

## Demo time!

---

# Часть 2. Рост сложности секции Arrange

---

## Сценарий 3. Данные от бизнес-аналитика в excel.

- БА предоставил данные и формулы в Excel
    - Формулы - для программирования логики
    - Данные - для проверки
- Стандартный подход для множественных данных:
    - Theory, InlineData, MemberData (в XUnit) 

```csharp
[Theory]
[InlineData(16.66, 6.25, ...)]
[InlineData(12.66, 5.45, ...)]
public void TestTableCase1(double value1, double value2, ..., double result)
{
    ...
}
```

--- 

MemberData полезен для генерации комбинаций (как пример, в лексерах)

```csharp
[Theory]
[MemberData(nameof(GetCheckTableData))]
public void TestTableCaseN(double value1, double value2, ..., double result)
{
    ...
}

private static IEnumerable<object[]> GetCheckTableData()
{
    /// динамически конструируем данные для проверки в отдельном методе
    foreach(...)
    {
        yield return new object[] { value1, value2, ..., valueResult }
    }
}
```

---

Excel - CSV - CSV-парсер

```csharp
public static IEnumerable<Transaction> ParseCsv(string filePath)
{
    var lines = File.ReadLines(filePath);

    foreach (var line in lines)
    {
        ...

        yield return new Transaction { ... };
    }
}

```

Да, но. Нужно переводить в формат csv или искать сторонние библиотеки.

---

## Работа с excel через F#.
Провайдер типа в F# позволяет производить парсинг excel в несколько строчек

- Встроенный провайдер типа в FSharp.Data
- В основном проекте можно подключить и использовать F#- проект, как и любой другой.
- Данные от БА, превращаются в короткий и наглядный тест 
    - Входные данные можно посмотреть в удобном табличном редакторе, 
    - Выходные - завести в approval в табличном формате
---

```fsharp
namespace FSharpExcelParser

open System
open System.Collections.Generic
open FSharp.Data
open Models

module ExcelParser =
    
    let parseExcel (filePath: string) : seq<Transaction> =
        let excel = ExcelFile.Load(filePath)
        excel.Data 
        |> Seq.map (fun row ->
            let transaction = Transaction()
            transaction.Id <- row.``Id``
            transaction.ParentId <- row.``ParentId``
            transaction.Date <- row.``Date``
            transaction.TransactionType <- row.``TransactionType``
            transaction.Funded <- row.``Funded``
            transaction.Unfunded <- row.``Unfunded``
            transaction)

    // вызываемый из C# метод
    [<CompiledName("ParseExcel")>]
    let parseExcelAsList (filePath: string) : IEnumerable<Transaction> =
        parseExcel filePath :> IEnumerable<Transaction>

```

---

# Часть 3. Интеграционные тесты и исправление бага

---

## Сценарий 4. Баг с прода

- С прода пришел баг, например:
    - сумма расходится с нужной на одну копейку. 
    - или какой-то из параметров принимает ошибочное значение. 

---

```json
Request:
{
	"value01": 100.1123,
	"value01": 32.1211,
	...
	"value10": 45.2341
}

Response:
[
	{
		"name": "w01",
		"result01": 0.01, <----- ошибка!
		...
		"result10": 0.08
	},
	...
	{
		"name": "w10",
		"result01": 4.31,
		...
		"result10": 1.88  
	}
]
```

---

- Зеленые тесты и 100%-е покрытие не гарантируют отсутствие багов. 

- "Pesticide paradox": набор тестов порождает ложное чувство безопасности и перестает ловить новые баги. 

- Поддерживать тесты в актуальном состоянии 
    - продакшн кейс внести в набор. 

- Нужен гибридный тест, он 
    - еще не является интеграционным, но уже и не юнит
    - для каких-то моков использовать реальный ввод, чтобы развернуть всю цепочку построения ответа.

- Текст запроса и текст результата - готовые данные для Arrange и Assert.
- Табличное форматирование может помочь визуализировать результат

---

```
Request:

value01  | value02 | ... | value10
----------------------------------
100.1123 | 32.1211 | ... | 45.2341

Response:

name | result01 | ... | result10
--------------------------------
w1   |     0.01 | ... |     0.08
...
w10  |     4.31 | ... |     1.88
```

---

## Demo time!

---

## Еще инструменты, Snapshot тестирование и Verify

- Verify - инструмент для снепшот-тестирования, аналогичный ApprovalTests
- Отличие снепшота и апрувал теста - это необходимость первоначального просмотра и подтверждения результата при апруве. 

---

| Аспект                  | **Verify**                                                             | **ApprovalTests**                                  |
| ----------------------- | ---------------------------------------------------------------------- | -------------------------------------------------- |
| **Концепция**           | Хранит **сгенерированный результат** как эталон                        | Хранит **явно утверждённый результат**             |
| **Рабочий процесс**     | Автоматически создаёт снапшот при первом запуске, затем сравнивает его | Требует ручного утверждения ожидаемого результата  |
| **Обработка изменений** | Можно автоматически обновлять снапшоты                                 | Изменения требуют явного подтверждения             |
| **Применение**          | JSON, логи, UI-рендеринг и др.                                         | Отчёты, форматированный текст, изображения и др.   |

---
### Проблемы и минусы аппрувал тестов

- лучше всего работают для детерминированных данных
- требуют внимательности при начальной проверке 
- немного медленнее, чем обычные ассерты
	- для огромной по размеру комбинации тестовых данных (например, сочетания токенов при разработке компилятора) лучше выбирать другие подходы
- рост количества текстовых файлов в репозитории
- стремление все даже в простых случаях делать аппрувалами
- разница настроек локализации сред выполнения. Тест может падать в контейнере и быть зеленым на локальной машине.

---


## Когда использовать 

| **Сценарий**                                    | **ApprovalTests**  | **Unit-тесты** |
| ------------------------------------------------| ------------------ | -------------- |
| Проверка части объекта                          | ✅                 | ✅            |
| Проверка точных значений                        | ✅                 | ✅            |
| Проверка больших объектов (JSON, таблицы, логи) | ✅                 | ❌            |
| Проверка **детерминированных** сложных данных   | ✅                 | ❌            |
| Динамические значения                           | ❌                 | ✅            |
| Вхождения строк, StartsWith, нужные байты, итд) | ❌                 | ✅            |

---

### Выводы

- Рост сложности юнит тестов с ростом сложности тестируемой логики, 
- Использование аппрувалов для 
    - уменьшения сложности 
    - ускорения написания тестов, 
    
- Использование табличного форматирования для удобного представления в diff-инструментах, 
- Парсинг excel при помощи F# для облегчения работы с данными секции Arrange, 
- Кейс комбинированного теста с данными продакшена.

---

## Спасибо за внимание!

- добавить ссылки на библиотеки
- ссылки на 