// Load the Suave library
#r "nuget: Suave"

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

let app =
    choose [
        path "/" >=> OK """
				<h1>Hello</h1>
				<h2>Goodbye</h2>
				"""
        path "/hello" >=> OK "Hello from Suave!"
    ]

// Start the web server
startWebServer defaultConfig app
