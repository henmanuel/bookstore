Public Class LoanRecord
    Public Property LoanID As Integer ' Un identificador único para cada préstamo
    Public Property BookID As Integer ' Relaciona el préstamo con un libro específico
    Public Property BorrowerUsername As String ' El usuario que tomó prestado el libro
    Public Property DateLoaned As DateTime
    Public Property DateDue As DateTime
    Public Property DateReturned As DateTime?

    ' Constructor para inicializar un registro de préstamo.
    Public Sub New(bookID As Integer, borrowerUsername As String, dateLoaned As DateTime, dateDue As DateTime)
        Me.BookID = bookID
        Me.BorrowerUsername = borrowerUsername
        Me.DateLoaned = dateLoaned
        Me.DateDue = dateDue
        Me.DateReturned = Nothing ' Al principio, el libro no ha sido devuelto
    End Sub

    ' Método para marcar el préstamo como devuelto
    Public Sub ReturnBook()
        Me.DateReturned = DateTime.Now
    End Sub
End Class
