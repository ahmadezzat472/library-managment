module Data

open System
open System.IO

//Record
type Book = {
    Id: int
    Title: string
    Author: string
    Genre: string
    Status: string
    BorrowDate: string option
}


let dataFilePath = "E:\Level 4\PL3\Project\librart-managment-v2\librart-managment-v2\Library data\library.txt"


let mutable books: Map<int, Book> = Map.empty


let loadBooks () =
    if File.Exists(dataFilePath) then
        let lines = File.ReadAllLines(dataFilePath)
        books <- 
            lines
            |> Array.map (fun line ->
                let parts = line.Split('|')
                let id = Int32.Parse(parts.[0])
                let title = parts.[1]
                let author = parts.[2]
                let genre = parts.[3]
                let status = parts.[4]
                let borrowDate = if parts.[5] = "N/A" then None else Some parts.[5]
                id, { Id = id; Title = title; Author = author; Genre = genre; Status = status; BorrowDate = borrowDate })
            |> Map.ofArray
    else
        books <- Map.empty


let saveBooks () =
    let lines = 
        books
        |> Map.toList
        |> List.map (fun (_, book) ->
            sprintf "%d|%s|%s|%s|%s|%s" 
                book.Id 
                book.Title 
                book.Author 
                book.Genre 
                book.Status 
                (book.BorrowDate |> Option.defaultValue "N/A"))
    File.WriteAllLines(dataFilePath, lines)
