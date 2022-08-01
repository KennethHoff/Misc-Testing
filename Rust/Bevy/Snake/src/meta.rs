use crate::{food, grid, snake};
use bevy::prelude::*;

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.add_event::<GameOverEvent>()
            .add_system(meta_input_system)
            .add_system_to_stage(CoreStage::Last, game_over_system);
    }
}
pub struct GameOverEvent;

fn meta_input_system(keyboard_input: Res<Input<KeyCode>>) {
    if keyboard_input.pressed(KeyCode::Q) {
        std::process::exit(0);
    }
}

fn game_over_system(
    segment_res: ResMut<snake::Segments>,
    segments_query: Query<Entity, With<snake::Segment>>,
    food_query: Query<Entity, With<food::Food>>,
    mut commands: Commands,
    mut game_over_reader: EventReader<GameOverEvent>,
    mut dimensions: ResMut<grid::Dimensions>,
) {
    if game_over_reader.iter().next().is_some() {
        for entity in food_query.iter().chain(segments_query.iter()) {
            commands.entity(entity).despawn();
        }
        snake::spawn(commands, segment_res);
        *dimensions = grid::Dimensions::default();
    }
}
