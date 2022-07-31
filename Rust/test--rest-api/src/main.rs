#[macro_use] extern crate rocket;

use rocket::{Build, Rocket};

#[get("/")]
fn index() -> &'static str {
    "Hello, world!"
}

#[launch]
fn rocket() -> Rocket<Build> {
    rocket::build().mount("/", routes![index])
}

#[get("/suppehue")]
fn suppehue() -> &'static str {
    // let latestSoup = reqwest::get("https://suppehue.vartdalen.com/api/soups/latest")?.text()?
    "latestSoup"
}