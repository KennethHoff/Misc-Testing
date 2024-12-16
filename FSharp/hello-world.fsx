let stuff = [ [ 1; 7; 3 ]; [ 1; 7; 6 ]; [ 3; 5; 12 ] ]

let thing a =
    a |> List.toSeq |> Seq.transpose |> Seq.map Seq.toList |> Seq.toList

let printatron e =
    Seq.iter
        (fun outer ->
            (Seq.iter (fun inner -> printf "%d " inner)) outer
            printfn "")
        e

printatron stuff
printfn ""

let stuff2 = thing stuff
printatron stuff2
printfn ""

let stuff3 = thing stuff2
printatron stuff3
printfn ""

let stuff4 = thing stuff3
printatron stuff4
printfn ""
