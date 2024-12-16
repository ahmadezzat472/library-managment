module LibraryOperations

open System
open Data


let addBook title author genre =
    loadBooks()
    let id = (books |> Map.toList |> List.length) + 1
    let book = { Id = id; Title = title; Author = author; Genre = genre; Status = "Available"; BorrowDate = None }
    books <- books.Add(id, book)
    saveBooks ()


let borrowBook id =
    loadBooks()
    match books.TryFind id with
    | Some(book) when book.Status = "Available" -> 
        let updatedBook = { book with Status = "Borrowed"; BorrowDate = Some(DateTime.Now.ToString("yyyy-MM-dd")) }
        books <- books.Add(id, updatedBook)
        saveBooks ()
        true
    | Some(_) -> false
    | None -> false


let returnBook id =
    loadBooks()
    match books.TryFind id with
    | Some(book) when book.Status = "Borrowed" -> 
        let updatedBook = { book with Status = "Available"; BorrowDate = None }
        books <- books.Add(id, updatedBook)
        saveBooks ()
        true
    | Some(_) -> false
    | None -> false


let searchBooks (title: string) =
    books
    |> Map.filter (fun _ book -> book.Title.ToLower().Contains(title.ToLower()))
    |> Map.toList
