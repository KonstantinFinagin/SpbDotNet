---
marp: true
---

# Approval тестирование в .net

---

## Константин Финагин

**Старший разработчик в Лаборатории Касперского (с 2023)**   
Ранее — старший разработчик в DataArt (с 2014)  

📧 konstantin.finagin@gmail.com

---

## О чём пойдёт речь?

- Юнит тесты, рост их сложности
- Как с этим бороться с помощью апрувал тестов
- Чем поможет форматирование данных в виде таблиц
- Уровень джуниор+ и выше

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

## 

---

## Demo time!

---


