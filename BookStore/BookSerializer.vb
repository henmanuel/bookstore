' Definir un módulo para la serialización y deserialización de libros.
Imports Newtonsoft.Json
Imports System.IO

Public Module BookSerializer
    ' Serializar una lista de libros en un archivo JSON llamado "books.json".
    Public Sub SerializeBooks(books As List(Of Book))
        Dim json As String = JsonConvert.SerializeObject(books, Formatting.Indented)
        Try
            File.WriteAllText("books.json", json)
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

    ' Deserializar datos de un archivo JSON en una lista de libros.
    Public Function DeserializeBooks() As List(Of Book)
        Dim path As String = "books.json"
        If Not File.Exists(path) Then
            Console.WriteLine("El archivo de libros no existe. Se creará una nueva lista vacía.")
            Return New List(Of Book)()
        End If

        Try
            Dim json As String = File.ReadAllText(path)
            Return JsonConvert.DeserializeObject(Of List(Of Book))(json)
        Catch ex As IOException
            Console.WriteLine($"Error al leer el archivo: {ex.Message}")
        Catch ex As JsonException
            Console.WriteLine($"Error de deserialización: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Un error inesperado ocurrió: {ex.Message}")
        End Try

        Return New List(Of Book)()
    End Function
End Module