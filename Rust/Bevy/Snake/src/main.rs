use bevy::log::{Level, LogSettings};
use bevy::prelude::*;
use bevy::window::PresentMode;

mod food;
mod grid;
mod snake;
mod meta;
mod camera;

const CLEAR: Color = Color::rgb(0.04, 0.04, 0.04);
const WINDOW_BASE_WIDTH: f32 = 500.;
const WINDOW_BASE_HEIGHT: f32 = 500.;

fn main() {
    let window_descriptor = WindowDescriptor {
        title: "Snake".to_string(),
        width: WINDOW_BASE_WIDTH,
        height: WINDOW_BASE_HEIGHT,
        present_mode: PresentMode::Fifo,
        ..default()
    };
    let log_settings = LogSettings {
        level: Level::INFO,
        ..default()
    };
    App::new()
        .insert_resource(window_descriptor)
        .insert_resource(log_settings)
        .insert_resource(ClearColor(CLEAR))
        .add_plugin(meta::Plugin)
        .add_plugin(grid::Plugin)
        .add_plugin(snake::Plugin)
        .add_plugin(food::Plugin)
        .add_plugin(camera::Plugin)
        .add_plugins(DefaultPlugins)
        .run();
}
