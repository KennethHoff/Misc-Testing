use bevy::core::FixedTimestep;
use bevy::prelude::*;

use crate::food::Food;
use crate::grid::{GridDimensions, GridDirection, GridPosition, GridSize};
use crate::meta::GameOverEvent;

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.insert_resource(SnakeSegments::default())
            .insert_resource(LastTailPosition::default())
            .add_event::<GrowthEvent>()
            .add_startup_system(spawning_system)
            .add_system_set(
                SystemSet::new()
                    .with_run_criteria(FixedTimestep::step(0.2))
                    .with_system(movement_system)
                    .with_system(eating_system.after(movement_system))
                    .with_system(growth_system.after(eating_system)),
            )
            .add_system(movement_input_system.before(movement_system));
    }
}

const SNAKE_HEAD_COLOR: Color = Color::rgb(0.7, 0.7, 0.7);
const SNAKE_SEGMENT_COLOR: Color = Color::rgb(0.5, 0.5, 0.5);

#[derive(Component)]
pub struct SnakeHead {
    pub movement_direction: GridDirection,
    pub input_direction: GridDirection,
}

#[derive(Component)]
pub struct SnakeSegment;

#[derive(Default, Deref, DerefMut)]
pub struct SnakeSegments(Vec<Entity>);

struct GrowthEvent;

#[derive(Default)]
struct LastTailPosition(Option<GridPosition>);
fn eating_system(
    mut commands: Commands,
    snake_head_query: Query<&GridPosition, With<SnakeHead>>,
    food_query: Query<(Entity, &GridPosition), With<Food>>,
    mut growth_writer: EventWriter<GrowthEvent>,
) {
    match snake_head_query.iter().next() {
        Some(head_pos) => {
            for (food_entity, food_pos) in food_query.iter() {
                if head_pos == food_pos {
                    commands.entity(food_entity).despawn();
                    growth_writer.send(GrowthEvent);
                }
            }
        }
        None => {
            info!("No head found");
        }
    }
}

fn growth_system(
    commands: Commands,
    last_tail_position: Res<LastTailPosition>,
    mut segments: ResMut<SnakeSegments>,
    mut growth_reader: EventReader<GrowthEvent>,
) {
    if let Some(_e) = growth_reader.iter().next() {
        let new_segment = spawn_segment(commands, last_tail_position.0.unwrap());
        segments.push(new_segment);
    };
}

fn movement_system(
    mut game_over_writer: EventWriter<GameOverEvent>,
    mut last_tail_position: ResMut<LastTailPosition>,
    segments: ResMut<SnakeSegments>,
    mut snake_head_query: Query<(Entity, &mut SnakeHead)>,
    mut position_query: Query<&mut GridPosition>,
    dimensions: Res<GridDimensions>,
) {
    if let Some((head_entity, mut head)) = snake_head_query.iter_mut().next() {
        let segment_positions = segments
            .iter()
            .map(|s| *position_query.get_mut(*s).unwrap())
            .collect::<Vec<GridPosition>>();

        let mut head_pos = position_query.get_mut(head_entity).unwrap();

        head.movement_direction = head.input_direction;

        match &head.movement_direction {
            GridDirection::Left => head_pos.x -= 1,
            GridDirection::Right => head_pos.x += 1,
            GridDirection::Up => head_pos.y += 1,
            GridDirection::Down => head_pos.y -= 1,
        };
        if head_pos.x < 0
            || head_pos.y < 0
            || head_pos.x as u32 > dimensions.width
            || head_pos.y as u32 > dimensions.height
        {
            info!("Out of bounds");
            game_over_writer.send(GameOverEvent);
        }
        if segment_positions.contains(&head_pos) {
            info!("Collided with itself");
            game_over_writer.send(GameOverEvent);
        }

        segment_positions
            .iter()
            .zip(segments.iter().skip(1))
            .for_each(|(pos, segment)| {
                *position_query.get_mut(*segment).unwrap() = *pos;
            });
        *last_tail_position = LastTailPosition(Some(*segment_positions.last().unwrap()));
    }
}

fn movement_input_system(keyboard_input: Res<Input<KeyCode>>, mut query: Query<&mut SnakeHead>) {
    if let Some(mut head) = query.iter_mut().next() {
        let dir: GridDirection =
            if keyboard_input.pressed(KeyCode::Left) || keyboard_input.pressed(KeyCode::A) {
                GridDirection::Left
            } else if keyboard_input.pressed(KeyCode::Down) || keyboard_input.pressed(KeyCode::S) {
                GridDirection::Down
            } else if keyboard_input.pressed(KeyCode::Up) || keyboard_input.pressed(KeyCode::W) {
                GridDirection::Up
            } else if keyboard_input.pressed(KeyCode::Right) || keyboard_input.pressed(KeyCode::D) {
                GridDirection::Right
            } else {
                head.input_direction
            };
        if head.movement_direction.opposite() != dir {
            head.input_direction = dir;
        }
    }
}

fn spawning_system(commands: Commands, segments: ResMut<SnakeSegments>) {
    spawn_snake(commands, segments);
}

pub fn spawn_snake(mut commands: Commands, mut segments: ResMut<SnakeSegments>) {
    let snake_head_sprite_bundle = SpriteBundle {
        sprite: Sprite {
            color: SNAKE_HEAD_COLOR,
            ..default()
        },
        transform: Transform {
            scale: Vec3::new(10.0, 10.0, 10.0),
            ..default()
        },
        ..default()
    };
    *segments = SnakeSegments(vec![
        commands
            .spawn_bundle(snake_head_sprite_bundle)
            .insert(GridPosition { x: 3, y: 3 })
            .insert(GridSize::square(0.8))
            .insert(SnakeHead {
                movement_direction: GridDirection::Up,
                input_direction: GridDirection::Up,
            })
            .insert(SnakeSegment)
            .id(),
        spawn_segment(commands, GridPosition { x: 3, y: 2 }),
    ]);
}

fn spawn_segment(mut commands: Commands, position: GridPosition) -> Entity {
    commands
        .spawn_bundle(SpriteBundle {
            sprite: Sprite {
                color: SNAKE_SEGMENT_COLOR,
                ..default()
            },
            ..default()
        })
        .insert(SnakeSegment)
        .insert(position)
        .insert(GridSize::square(0.65))
        .id()
}
