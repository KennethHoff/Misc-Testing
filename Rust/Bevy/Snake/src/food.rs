use crate::{grid, snake};
use bevy::core::FixedTimestep;
use bevy::prelude::*;

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.add_system_set(
            SystemSet::new()
                .with_run_criteria(FixedTimestep::step(1.0))
                .with_system(food_spawning_system),
        );
    }
}

const FOOD_COLOR: Color = Color::rgb(1.0, 0.0, 1.0);

#[derive(Component)]
pub struct Food;

fn food_spawning_system(
    mut commands: Commands,
    segments: ResMut<snake::Segments>,
    mut query: Query<&mut grid::Position>,
    dimensions: Res<grid::Dimensions>,
) {
    commands
        .spawn_bundle(SpriteBundle {
            sprite: Sprite {
                color: FOOD_COLOR,
                ..default()
            },
            ..default()
        })
        .insert(Food)
        .insert(get_random_valid_position(segments, &mut query, dimensions))
        .insert(grid::Size::square(0.5));
}

fn get_random_valid_position(
    segments: ResMut<snake::Segments>,
    query: &mut Query<&mut grid::Position>,
    dimensions: Res<grid::Dimensions>,
) -> grid::Position {
    let occupied_positions = segments
        .iter()
        .map(|s| *query.get_mut(*s).unwrap())
        .collect::<Vec<grid::Position>>();

    let mut new_position = get_random_position(&dimensions);
    while occupied_positions.contains(&new_position) {
        new_position = get_random_position(&dimensions);
    }
    new_position
}

fn get_random_position(dimensions: &Res<grid::Dimensions>) -> grid::Position {
    grid::Position {
        x: (rand::random::<f32>() * dimensions.width as f32) as i32,
        y: (rand::random::<f32>() * dimensions.height as f32) as i32,
    }
}
