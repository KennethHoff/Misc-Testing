type ThingyMaJigg = Red of string | Blue of int

// Define a function that takes an integer and returns its square
let square x =
  match x with
  | Red red -> red
  | Blue blue -> (blue * blue).ToString();

let thingy = 5;

// Print the square of 5
printfn "The square of %d is: %s" thingy (square (Blue thingy))

// A simple loop that prints numbers from 1 to 5
for i in 1..5 do
    printfn "Number: %d" i
