Imports Newtonsoft.Json
Imports System.IO

Public Module LoanRecordSerializer
    Private Const LoanRecordFilePath As String = "loanRecords.json"

    ' Serializar la lista de registros de préstamos en un archivo JSON.
    Public Sub SerializeLoanRecords(loanRecords As List(Of LoanRecord))
        Dim json As String = JsonConvert.SerializeObject(loanRecords, Formatting.Indented)
        Try
            File.WriteAllText(LoanRecordFilePath, json)
        Catch ex As IOException
            Console.WriteLine($"Error de IO al intentar escribir en el archivo: {ex.Message}")
        Catch ex As UnauthorizedAccessException
            Console.WriteLine($"Acceso no autorizado al archivo: {ex.Message}")
        Catch ex As JsonException
            Console.WriteLine($"Error de serialización JSON: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Error inesperado: {ex.Message}")
        End Try
    End Sub

    ' Deserializar la lista de registros de préstamos desde un archivo JSON.
    Public Function DeserializeLoanRecords() As List(Of LoanRecord)
        If Not File.Exists(LoanRecordFilePath) Then
            Console.WriteLine("El archivo de registros de préstamos no existe. Se creará una nueva lista vacía.")
            Return New List(Of LoanRecord)()
        End If

        Try
            Dim json As String = File.ReadAllText(LoanRecordFilePath)
            Return JsonConvert.DeserializeObject(Of List(Of LoanRecord))(json)
        Catch ex As IOException
            Console.WriteLine($"Error al leer el archivo: {ex.Message}")
        Catch ex As JsonException
            Console.WriteLine($"Error de deserialización: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Un error inesperado ocurrió: {ex.Message}")
            Return New List(Of LoanRecord)()
        End Try
    End Function
End Module
