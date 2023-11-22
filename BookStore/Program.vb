' El módulo principal de la aplicación que contiene el menú y los métodos de interacción del usuario.
Module Program
    ' Declara las listas como variables globales al principio del módulo.
    Dim users As List(Of User) = DeserializeUsers()
    Dim books As List(Of Book) = DeserializeBooks()
    Dim loanRecords As New List(Of LoanRecord)

    ' Método de autenticación
    Public Function Authenticate(username As String, password As String, ByRef userList As List(Of User)) As User
        For Each user In userList
            If user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) AndAlso user.VerifyPassword(password) Then
                Return user
            End If
        Next

        Return Nothing
    End Function

    Sub Main()
        ' Buscar al usuario "admin" en la lista de usuarios
        Dim adminUser As User = users.FirstOrDefault(Function(u) u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))

        ' Si el usuario "admin" no existe, crearlo con una contraseña predeterminada
        If adminUser Is Nothing Then
            ' Crear el usuario "admin" con contraseña predeterminada
            adminUser = New Librarian("admin", "admin123")
            ' Agregar el usuario "admin" a la lista de usuarios
            users.Add(adminUser)
            ' Serializar la lista de usuarios actualizada
            SerializeUsers(users)
        End If

        Dim currentUser As User = Nothing

        While True
            Console.WriteLine("Por favor, inicie sesión.")
            Console.Write("Usuario: ")
            Dim username As String = Console.ReadLine()
            Console.Write("Contraseña: ")
            Dim password As String = Console.ReadLine()

            ' Autenticar al usuario ingresado
            currentUser = Authenticate(username, password, users)

            If currentUser IsNot Nothing Then
                Console.WriteLine($"Bienvenido {currentUser.Username}!")

                If TypeOf currentUser Is Librarian Then
                    ShowLibrarianMenu(currentUser)
                ElseIf TypeOf currentUser Is Student Then
                    ShowStudentMenu(currentUser)
                End If

                ' Una vez que se muestra el menú y se completa, se debe cerrar la sesión del usuario actual
                currentUser = Nothing
            Else
                Console.WriteLine("Inicio de sesión incorrecto. Inténtelo de nuevo.")
            End If
        End While
    End Sub

    Private Sub ShowStudentMenu(currentUser As User)
        Dim menuOption As String = ""
        While menuOption <> "3"
            Console.WriteLine("1. Buscar Libros")
            Console.WriteLine("2. Reservar Libro")
            Console.WriteLine("3. Salir")
            Console.Write("Seleccione una opción: ")
            menuOption = Console.ReadLine()

            Select Case menuOption
                Case "1"
                    SearchBooks()
                Case "2"
                    ReserveBook()
                Case "3"
                    Console.WriteLine("Cerrando sesión de estudiante...")
                    Exit While
                Case Else
                    Console.WriteLine("Opción no válida, intente de nuevo.")
            End Select
        End While
    End Sub

    Private Sub ShowLibrarianMenu(currentUser As User)
        Dim menuOption As String = ""
        While menuOption <> "7"
            Console.WriteLine("1. Mostrar Libros")
            Console.WriteLine("2. Agregar Libro")
            Console.WriteLine("3. Eliminar Libro")
            Console.WriteLine("4. Agregar Usuario")
            Console.WriteLine("5. Prestar Libro")
            Console.WriteLine("6. Devolver Libro")
            Console.WriteLine("7. Salir")
            Console.Write("Seleccione una opción: ")
            menuOption = Console.ReadLine()

            Select Case menuOption
                Case "1"
                    DisplayBooks()
                Case "2"
                    InsertBook()
                Case "3"
                    RemoveBook()
                Case "4"
                    AddUser()
                Case "5"
                    LoanBook()
                Case "6"
                    ReturnBook()
                Case "7"
                    Console.WriteLine("Cerrando sesión de bibliotecario...")
                    Exit While
                Case Else
                    Console.WriteLine("Opción no válida, intente de nuevo.")
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

                ' Verificar si ya existe un libro con el mismo título
                If books.Any(Function(b) b.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) Then
                    Console.WriteLine("Error: Ya existe un libro con este título.")
                    Return
                End If

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

    Private Sub SearchBooks()
        Console.Write("Ingrese el título del libro a buscar: ")
        Dim searchTitle As String = Console.ReadLine().ToLower()
        Dim books As List(Of Book) = DeserializeBooks()
        Dim foundBooks As IEnumerable(Of Book) = books.Where(Function(b) b.Title.ToLower().Contains(searchTitle))

        If foundBooks.Any() Then
            For Each book In foundBooks
                Console.WriteLine(book.ToString())
            Next
        Else
            Console.WriteLine("No se encontraron libros con ese título.")
        End If
    End Sub

    Private Sub ReserveBook()
        Console.Write("Ingrese el título del libro a reservar: ")
        Dim reserveTitle As String = Console.ReadLine().ToLower()
        Dim books As List(Of Book) = DeserializeBooks()
        Dim bookToReserve As Book = books.FirstOrDefault(Function(b) b.Title.ToLower().Equals(reserveTitle) AndAlso b.Status.Equals("Available"))

        If bookToReserve IsNot Nothing Then
            bookToReserve.Status = "Reserved"
            SerializeBooks(books)
            Console.WriteLine("Libro reservado con éxito.")
        Else
            Console.WriteLine("El libro no está disponible para reservar o no existe.")
        End If
    End Sub

    Private Sub RemoveBook()
        Console.Write("Ingrese el título del libro a eliminar: ")
        Dim removeTitle As String = Console.ReadLine().ToLower()
        Dim books As List(Of Book) = DeserializeBooks()
        Dim bookToRemove As Book = books.FirstOrDefault(Function(b) b.Title.ToLower().Equals(removeTitle))

        If bookToRemove IsNot Nothing Then
            books.Remove(bookToRemove)
            SerializeBooks(books)
            Console.WriteLine("Libro eliminado con éxito.")
        Else
            Console.WriteLine("No se encontró el libro a eliminar.")
        End If
    End Sub

    Private Sub AddUser()
        Console.WriteLine("Agregar nuevo usuario")
        Console.Write("Ingrese el tipo de usuario (bibliotecario/estudiante): ")
        Dim userType As String = Console.ReadLine().ToLower()
        Console.Write("Ingrese el nombre de usuario: ")
        Dim username As String = Console.ReadLine()
        Console.Write("Ingrese la contraseña: ")
        Dim password As String = Console.ReadLine()

        Dim newUser As User = Nothing
        If userType = "bibliotecario" Then
            newUser = New Librarian(username, password)
        ElseIf userType = "estudiante" Then
            newUser = New Student(username, password)
        Else
            Console.WriteLine("Tipo de usuario no válido.")
            Return
        End If

        Dim users As List(Of User) = UserSerializer.DeserializeUsers()
        If users.Any(Function(u) u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) Then
            Console.WriteLine("El nombre de usuario ya existe.")
        Else
            users.Add(newUser)
            UserSerializer.SerializeUsers(users)
            Console.WriteLine("Usuario agregado con éxito.")
        End If
    End Sub

    Private Sub LoanBook()
        Console.Write("Ingrese el título del libro a prestar: ")
        Dim loanTitle As String = Console.ReadLine().ToLower()

        Dim bookToLoan As Book = books.FirstOrDefault(Function(b) b.Title.ToLower().Equals(loanTitle) AndAlso b.Status = "Available")

        If bookToLoan IsNot Nothing Then
            Console.Write("Ingrese el nombre de usuario del estudiante a quien se le prestará el libro: ")
            Dim borrowerUsername As String = Console.ReadLine()

            ' Verifica si el usuario es un estudiante
            Dim borrower As User = users.FirstOrDefault(Function(u) u.Username.ToLower() = borrowerUsername.ToLower() AndAlso TypeOf u Is Student)

            If borrower IsNot Nothing Then
                bookToLoan.Status = "Loaned"

                Dim newLoanRecord As New LoanRecord(bookToLoan.BookID, borrowerUsername, DateTime.Now, DateTime.Now.AddDays(14)) ' Supongamos que el préstamo es por 14 días

                loanRecords.Add(newLoanRecord)

                SerializeBooks(books)
                SerializeLoanRecords(loanRecords)

                Console.WriteLine($"El libro '{bookToLoan.Title}' ha sido prestado a {borrowerUsername}.")
            Else
                Console.WriteLine("No se encontró un estudiante con ese nombre de usuario.")
            End If
        Else
            Console.WriteLine("El libro no está disponible para ser prestado o no existe.")
        End If
    End Sub

    Private Sub ReturnBook()
        Console.Write("Ingrese el título del libro a devolver: ")
        Dim returnTitle As String = Console.ReadLine().ToLower()

        Dim bookToReturn As Book = books.FirstOrDefault(Function(b) b.Title.ToLower().Equals(returnTitle) AndAlso b.Status = "Loaned")

        If bookToReturn IsNot Nothing Then
            Console.Write("Ingrese el nombre de usuario del estudiante que devuelve el libro: ")
            Dim borrowerUsername As String = Console.ReadLine()

            ' Verifica si el usuario es un estudiante
            Dim borrower As User = users.FirstOrDefault(Function(u) u.Username.ToLower() = borrowerUsername.ToLower() AndAlso TypeOf u Is Student)

            If borrower IsNot Nothing Then
                ' Verifica si existe un registro de préstamo abierto para el libro
                Dim loanRecordToClose As LoanRecord = loanRecords.FirstOrDefault(Function(lr) lr.BookID = bookToReturn.BookID AndAlso lr.BorrowerUsername.ToLower() = borrowerUsername.ToLower() AndAlso lr.DateReturned Is Nothing)

                If loanRecordToClose IsNot Nothing Then
                    bookToReturn.Status = "Available"
                    loanRecordToClose.DateReturned = DateTime.Now

                    SerializeBooks(books)
                    SerializeLoanRecords(loanRecords)

                    Console.WriteLine($"El libro '{bookToReturn.Title}' ha sido devuelto.")
                Else
                    Console.WriteLine("No se encontró un registro de préstamo abierto para este libro con el usuario especificado.")
                End If
            Else
                Console.WriteLine("No se encontró un estudiante con ese nombre de usuario.")
            End If
        Else
            Console.WriteLine("El libro no está prestado o no existe.")
        End If
    End Sub

End Module