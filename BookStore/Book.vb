' Importar las bibliotecas necesarias para la serialización JSON y el manejo de archivos.
Imports Newtonsoft.Json
Imports System.IO

' Definir la clase Book con propiedades para título, autor y año de publicación.
Public Class Book
    Public Property BookID As Integer
    Public Property Title As String
    Public Property Author As String
    Public Property Year As Integer
    Public Property Status As String = "Available"

    ' Inicializar un nuevo libro con las propiedades dadas y realizar validaciones.
    Public Sub New(title As String, author As String, year As Integer)
        If String.IsNullOrEmpty(title) Then
            Throw New ArgumentException("El título no puede estar vacío")
        End If
        If String.IsNullOrEmpty(author) Then
            Throw New ArgumentException("El autor no puede estar vacío")
        End If
        If year <= 0 Then
            Throw New ArgumentException("El año de publicación debe ser un número positivo")
        End If

        Me.Title = title
        Me.Author = author
        Me.Year = year
    End Sub

    ' Método ToString para mostrar la información del libro.
    Public Overrides Function ToString() As String
        Return $"Título: {Title}, Autor: {Author}, Año de Publicación: {Year}"
    End Function
End Class