Imports Newtonsoft.Json
Imports System.IO

Public Module BookSerializer
    Private Const BookFilePath As String = "books.json"

    Public Sub SerializeBooks(books As List(Of Book))
        ' Verificar si hay títulos de libros duplicados antes de la serialización
        If books.GroupBy(Function(b) b.Title.Trim().ToLower()).Any(Function(g) g.Count() > 1) Then
            Console.WriteLine("Error: Se encontraron títulos de libros duplicados. No se puede serializar la lista de libros.")
            Return
        End If

        ' Validar los libros antes de la serialización
        If books.Any(Function(b) String.IsNullOrEmpty(b.Title) OrElse String.IsNullOrEmpty(b.Author) OrElse b.Year <= 0) Then
            Console.WriteLine("No se puede serializar: uno o más libros tienen datos inválidos.")
            Return
        End If

        Dim json As String = JsonConvert.SerializeObject(books, Formatting.Indented)
        Try
            File.WriteAllText(BookFilePath, json)
        Catch ex As Exception
            Console.WriteLine($"Error al escribir en el archivo '{BookFilePath}': {ex.Message}")
        End Try
    End Sub

    Public Function DeserializeBooks() As List(Of Book)
        If Not File.Exists(BookFilePath) Then
            Console.WriteLine($"El archivo '{BookFilePath}' no existe. Se creará una nueva lista vacía.")
            Return New List(Of Book)()
        End If

        Try
            Dim json As String = File.ReadAllText(BookFilePath)
            If String.IsNullOrWhiteSpace(json) Then
                Console.WriteLine($"El archivo '{BookFilePath}' está vacío. Se creará una nueva lista vacía.")
                Return New List(Of Book)()
            End If

            Dim books As List(Of Book) = JsonConvert.DeserializeObject(Of List(Of Book))(json)
            If books Is Nothing Then Return New List(Of Book)()

            ' Validar los libros deserializados
            Return books.Where(Function(b) Not String.IsNullOrEmpty(b.Title) AndAlso Not String.IsNullOrEmpty(b.Author) AndAlso b.Year > 0).ToList()
        Catch ex As JsonException
            Console.WriteLine($"Error de deserialización en el archivo '{BookFilePath}': {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Un error inesperado ocurrió al leer el archivo '{BookFilePath}': {ex.Message}")
        End Try

        Return New List(Of Book)()
    End Function
End Module
