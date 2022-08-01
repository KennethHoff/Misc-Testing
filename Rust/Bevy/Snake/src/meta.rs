use bevy::prelude::*;

use crate::food::Food;
use crate::grid::GridDimensions;
use crate::snake::{spawn_snake, SnakeSegment, SnakeSegments};

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.add_event::<GameOverEvent>()
            .add_system(game_input_meta)
            .add_system_to_stage(CoreStage::Last, game_over);
    }
}
pub struct GameOverEvent;

fn game_input_meta(keyboard_input: Res<Input<KeyCode>>) {
    if keyboard_input.pressed(KeyCode::Q) {
        std::process::exit(0);
    }
}

fn game_over(
    segment_res: ResMut<SnakeSegments>,
    snake_segments: Query<Entity, With<SnakeSegment>>,
    food_items: Query<Entity, With<Food>>,
    mut commands: Commands,
    mut game_over_reader: EventReader<GameOverEvent>,
    mut dimensions: ResMut<GridDimensions>,
) {
    if game_over_reader.iter().next().is_some() {
        for entity in food_items.iter().chain(snake_segments.iter()) {
            commands.entity(entity).despawn();
        }
        spawn_snake(commands, segment_res);
        *dimensions = GridDimensions::default();
    }
}
