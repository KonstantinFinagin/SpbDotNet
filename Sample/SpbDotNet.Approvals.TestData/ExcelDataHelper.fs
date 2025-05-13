namespace SpbDotNet.Approvals.TestData

module ExcelDataHelper =

    open FSharp.Interop.Excel
    open SpbDotNet.Approvals.Model

    [<Literal>]
    let private samplePath = __SOURCE_DIRECTORY__ + "/DataSamples/InputData.xlsx"

    type InputDataExcel = ExcelFile<samplePath>

    let private inputDataRows (sheet: InputDataExcel) =
        sheet.Data
        |> Seq.map (fun row -> InputData((int)row.UserId, row.RawInput))

    let GetInputDataItems (path: string) =
        let file = new InputDataExcel(path)
        inputDataRows file |> Seq.toArray