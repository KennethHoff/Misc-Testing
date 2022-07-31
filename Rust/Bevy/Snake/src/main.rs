use bevy::log::{Level, LogSettings};
use bevy::prelude::*;
use bevy::window::PresentMode;
use crate::camera::CameraPlugin;

use crate::food::FoodPlugin;
use crate::grid::GridPlugin;
use crate::snake::SnakePlugin;
use crate::systems::MetaPlugin;

mod food;
mod grid;
mod snake;
mod systems;
mod camera;

const CLEAR: Color = Color::rgb(0.04, 0.04, 0.04);
const WINDOW_BASE_WIDTH: f32 = 500.;
const WINDOW_BASE_HEIGHT: f32 = 500.;

fn main() {

    App::new()
        .insert_resource(WindowDescriptor {
            title: "Snake".to_string(),
            width: WINDOW_BASE_WIDTH,
            height: WINDOW_BASE_HEIGHT,
            present_mode: PresentMode::Fifo,
            ..default()
        })
        .insert_resource(LogSettings {
            level: Level::INFO,
            ..default()
        })
        .insert_resource(ClearColor(CLEAR))
        .add_plugin(MetaPlugin)
        .add_plugin(GridPlugin)
        .add_plugin(SnakePlugin)
        .add_plugin(FoodPlugin)
        .add_plugin(CameraPlugin)
        .add_plugins(DefaultPlugins)
        .run();
}
