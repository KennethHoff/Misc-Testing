use bevy::prelude::*;

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.add_system_set_to_stage(
            CoreStage::PostUpdate,
            SystemSet::new()
                .with_system(position_translation)
                .with_system(size_scaling),
        )
        .insert_resource(Dimensions::default());
    }
}

const GRID_WIDTH_START: u32 = 10;
const GRID_HEIGHT_START: u32 = 10;

pub struct Dimensions {
    pub width: u32,
    pub height: u32,
}

impl Default for Dimensions {
    fn default() -> Self {
        Self {
            width: GRID_WIDTH_START,
            height: GRID_HEIGHT_START,
        }
    }
}

#[derive(Component)]
pub struct Size {
    pub width: f32,
    pub height: f32,
}

impl Size {
    pub const fn square(size: f32) -> Self {
        Self {
            width: size,
            height: size,
        }
    }
}

#[derive(Component, Copy, Clone, Eq, PartialEq)]
pub struct Position {
    pub x: i32,
    pub y: i32,
}

#[derive(PartialEq, Eq, Copy, Clone)]
pub enum Direction {
    Up,
    Down,
    Left,
    Right,
}

impl Direction {
    pub const fn opposite(self) -> Self {
        match self {
            Self::Up => Self::Down,
            Self::Down => Self::Up,
            Self::Left => Self::Right,
            Self::Right => Self::Left,
        }
    }
}

fn size_scaling(
    windows: Res<Windows>,
    mut query: Query<(&mut Transform, &Size)>,
    dimensions: Res<Dimensions>,
) {
    match windows.get_primary() {
        Some(window) => {
            for (mut transform, sprite_size) in query.iter_mut() {
                transform.scale = Vec3::new(
                    sprite_size.width / dimensions.width as f32 * window.width(),
                    sprite_size.height / dimensions.height as f32 * window.height(),
                    1.0,
                );
            }
        }
        None => {
            info!("No window found");
        }
    };
}

fn position_translation(
    windows: Res<Windows>,
    mut query: Query<(&mut Transform, &Position)>,
    dimensions: Res<Dimensions>,
) {
    fn convert(pos: f32, bound_window: f32, bound_game: f32) -> f32 {
        let tile_size = bound_window / bound_game;
        pos / bound_game * bound_window - (bound_window / 2.) + (tile_size / 2.)
    }
    match windows.get_primary() {
        Some(window) => {
            for (mut transform, pos) in query.iter_mut() {
                transform.translation = Vec3::new(
                    convert(pos.x as f32, window.width() as f32, dimensions.width as f32),
                    convert(
                        pos.y as f32,
                        window.height() as f32,
                        dimensions.height as f32,
                    ),
                    0.0,
                );
            }
        }
        None => {
            info!("No window found");
        }
    }
}
