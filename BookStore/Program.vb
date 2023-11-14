' El módulo principal de la aplicación que contiene el menú y los métodos de interacción del usuario.
Module Program
    Sub Main()
        Dim menuOption As String = ""

        ' Bucle principal de la aplicación.
        While menuOption <> "3"
            Console.WriteLine("1. Mostrar Libros")
            Console.WriteLine("2. Insertar Libro")
            Console.WriteLine("3. Salir")
            Console.Write("Seleccione una opción: ")
            menuOption = Console.ReadLine()

            Select Case menuOption
                Case "1"
                    DisplayBooks()
                Case "2"
                    InsertBook()
                Case "3"
                    ' Salir de la aplicación.
                Case Else
                    Console.WriteLine("Opción no válida.")
            End Select
        End While
    End Sub

    ' Método para mostrar los libros disponibles.
    Sub DisplayBooks()
        Console.WriteLine("Libros disponibles:")
        Dim books As List(Of Book) = DeserializeBooks()

        If books.Count = 0 Then
            Console.WriteLine("No hay libros disponibles.")
        Else
            For Each book In books
                Console.WriteLine(book.ToString())
            Next
        End If
    End Sub

    ' Método para insertar un nuevo libro pidiendo al usuario la información necesaria.
    Sub InsertBook()
        Console.Write("Ingrese el título del libro: ")
        Dim title As String = Console.ReadLine()
        Console.Write("Ingrese el autor del libro: ")
        Dim author As String = Console.ReadLine()
        Console.Write("Ingrese el año de publicación: ")
        Dim yearInput As String = Console.ReadLine()
        Dim year As Integer

        If Integer.TryParse(yearInput, year) AndAlso year > 0 Then
            Try
                Dim newBook As New Book(title, author, year)
                Dim books As List(Of Book) = DeserializeBooks()
                books.Add(newBook)
                SerializeBooks(books)
                Console.WriteLine("Libro agregado correctamente.")
            Catch ex As ArgumentException
                Console.WriteLine($"Error al crear el libro: {ex.Message}")
            End Try
        Else
            Console.WriteLine("El año de publicación ingresado no es válido. Debe ser un número entero positivo.")
        End If
    End Sub
End Module