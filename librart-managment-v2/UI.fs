module UI

open System
open System.Windows.Forms
open Data
open LibraryOperations

// Populate the DataGridView with books
let populateTable (table: DataGridView) =
    table.Rows.Clear()
    loadBooks()
    books |> Map.iter (fun _ book ->
        table.Rows.Add([| box book.Id; box book.Title; box book.Author; box book.Genre; box book.Status; box (book.BorrowDate |> Option.defaultValue "N/A") |]) |> ignore)

// Main function to run the UI
[<STAThread>]
[<EntryPoint>]
let main argv = 
    // Load books from file
    loadBooks ()

    // Main form
    let form = new Form(Text = "Library Management System", Width = 800, Height = 600)

    // DataGridView for displaying books
    let bookTable = new DataGridView(Top = 20, Left = 20, Width = 740, Height = 300, ReadOnly = true, AllowUserToAddRows = false)
    bookTable.ColumnCount <- 6
    bookTable.Columns.[0].Name <- "ID"
    bookTable.Columns.[1].Name <- "Title"
    bookTable.Columns.[2].Name <- "Author"
    bookTable.Columns.[3].Name <- "Genre"
    bookTable.Columns.[4].Name <- "Status"
    bookTable.Columns.[5].Name <- "Borrow Date"
    populateTable bookTable
    form.Controls.Add(bookTable)

    // Add book section
    let titleLabel = new Label(Text = "Title:", AutoSize = true, Top = 340, Left = 20)
    let titleInput = new TextBox(Top = 340, Left = 100, Width = 200)
    let authorLabel = new Label(Text = "Author:", AutoSize = true, Top = 380, Left = 20)
    let authorInput = new TextBox(Top = 380, Left = 100, Width = 200)
    let genreLabel = new Label(Text = "Genre:", AutoSize = true, Top = 420, Left = 20)
    let genreInput = new TextBox(Top = 420, Left = 100, Width = 200)
    let addButton = new Button(Text = "Add Book", Top = 460, Left = 100)
    addButton.Click.Add(fun _ ->
        match (titleInput.Text, authorInput.Text, genreInput.Text) with
        | ("", _, _) | (_, "", _) | (_, _, "") -> 
            MessageBox.Show("Please fill all fields!") |> ignore
        | (title, author, genre) -> 
            addBook title author genre
            populateTable bookTable
            titleInput.Clear()
            authorInput.Clear()
            genreInput.Clear()
            MessageBox.Show("Book added successfully!") |> ignore
    )

    form.Controls.AddRange([| titleLabel; titleInput; authorLabel; authorInput; genreLabel; genreInput; addButton |])


    // Borrow book section
    let borrowLabel = new Label(Text = "Borrow Book ID:", AutoSize = true, Top = 340, Left = 340)
    let borrowInput = new TextBox(Top = 340, Left = 450, Width = 100)
    let borrowButton = new Button(Text = "Borrow", Top = 340, Left = 560)
    borrowButton.Click.Add(fun _ ->
        match Int32.TryParse(borrowInput.Text) with
        | (true, id) ->
            if borrowBook id then
                populateTable bookTable
                borrowInput.Clear()
                MessageBox.Show("Book borrowed successfully!") |> ignore
            else
                populateTable bookTable
                borrowInput.Clear()
                MessageBox.Show("Book is already borrowed!") |> ignore
        | _ -> MessageBox.Show("Invalid Book ID!") |> ignore)
    form.Controls.AddRange([| borrowLabel; borrowInput; borrowButton |])

    // Return book section
    let returnLabel = new Label(Text = "Return Book ID:", AutoSize = true, Top = 380, Left = 340)
    let returnInput = new TextBox(Top = 380, Left = 450, Width = 100)
    let returnButton = new Button(Text = "Return", Top = 380, Left = 560)
    returnButton.Click.Add(fun _ ->
        match Int32.TryParse(returnInput.Text) with
        | (true, id) ->
            if returnBook id then
                populateTable bookTable
                returnInput.Clear()
                MessageBox.Show("Book returned successfully!") |> ignore
            else
                populateTable bookTable
                returnInput.Clear()
                MessageBox.Show("Book cannot be returned!") |> ignore
        | _ -> MessageBox.Show("Invalid Book ID!") |> ignore)
    form.Controls.AddRange([| returnLabel; returnInput; returnButton |])

    // Search book section
    let searchLabel = new Label(Text = "Search by Title:", AutoSize = true, Top = 420, Left = 340)
    let searchInput = new TextBox(Top = 420, Left = 450, Width = 200)
    let searchButton = new Button(Text = "Search", Top = 420, Left = 660)
    searchButton.Click.Add(fun _ ->
        let results = searchBooks searchInput.Text
        bookTable.Rows.Clear()
        results |> List.iter (fun (_, book) ->
            bookTable.Rows.Add([| box book.Id; box book.Title; box book.Author; box book.Genre; box book.Status; box (book.BorrowDate |> Option.defaultValue "N/A") |]) |> ignore)
        if results.IsEmpty then MessageBox.Show("No books found!") |> ignore)
    form.Controls.AddRange([| searchLabel; searchInput; searchButton |])

    // Run the form
    Application.Run(form)
    0