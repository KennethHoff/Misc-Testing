use bevy::prelude::*;

pub struct Plugin;

impl bevy::prelude::Plugin for Plugin {
    fn build(&self, app: &mut App) {
        app.add_system_set_to_stage(
            CoreStage::PostUpdate,
            SystemSet::new()
                .with_system(position_translation)
                .with_system(size_scaling)
        )
        .insert_resource(GridDimensions::default());
    }
}

const GRID_WIDTH_START: u32 = 10;
const GRID_HEIGHT_START: u32 = 10;
const GRID_SECONDS_BETWEEN_INCREASES: f64 = 2.;

pub struct GridDimensions {
    pub width: u32,
    pub height: u32,
    internal_width_scalar: f64,
    internal_height_scalar: f64,
}

impl Default for GridDimensions {
    fn default() -> Self {
        GridDimensions {
            width: GRID_WIDTH_START,
            height: GRID_HEIGHT_START,
            internal_width_scalar: GRID_WIDTH_START as f64,
            internal_height_scalar: GRID_HEIGHT_START as f64,
        }
    }
}

fn grid_dimension_increase(mut dimensions: ResMut<GridDimensions>, time: Res<Time>) {
    let change_this_tick = time.delta_seconds_f64() / GRID_SECONDS_BETWEEN_INCREASES;
    dimensions.internal_width_scalar += change_this_tick;
    dimensions.internal_height_scalar += change_this_tick;
    let floored_internal_width = dimensions.internal_width_scalar as u32;
    let floored_internal_height = dimensions.internal_height_scalar as u32;

    let width_should_change = floored_internal_width != dimensions.width;
    if width_should_change {
        dimensions.width = floored_internal_width;
    }
    let height_should_change = floored_internal_height != dimensions.height;
    if height_should_change {
        dimensions.height = floored_internal_height;
    }
    if width_should_change || height_should_change {
        info!(
            "Grid dimensions changed to {:?}x{:?}",
            dimensions.width, dimensions.height
        );
    }
}

#[derive(Component)]
pub struct GridSize {
    pub width: f32,
    pub height: f32,
}

#[derive(Component, Copy, Clone, Eq, PartialEq)]
pub struct GridPosition {
    pub x: i32,
    pub y: i32,
}

impl GridSize {
    pub fn square(size: f32) -> Self {
        Self {
            width: size,
            height: size,
        }
    }
}

#[derive(PartialEq, Copy, Clone)]
pub enum GridDirection {
    Up,
    Down,
    Left,
    Right,
}

impl GridDirection {
    pub fn opposite(self) -> Self {
        match self {
            GridDirection::Up => GridDirection::Down,
            GridDirection::Down => GridDirection::Up,
            GridDirection::Left => GridDirection::Right,
            GridDirection::Right => GridDirection::Left,
        }
    }
}

fn size_scaling(
    windows: Res<Windows>,
    mut entities: Query<(&mut Transform, &GridSize)>,
    dimensions: Res<GridDimensions>,
) {
    match windows.get_primary() {
        Some(window) => {
            for (mut transform, sprite_size) in entities.iter_mut() {
                transform.scale = Vec3::new(
                    sprite_size.width / dimensions.width as f32 * window.width() as f32,
                    sprite_size.height / dimensions.height as f32 * window.height() as f32,
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
    mut entities: Query<(&mut Transform, &GridPosition)>,
    dimensions: Res<GridDimensions>,
) {
    fn convert(pos: f32, bound_window: f32, bound_game: f32) -> f32 {
        let tile_size = bound_window / bound_game;
        pos / bound_game * bound_window - (bound_window / 2.) + (tile_size / 2.)
    }
    match windows.get_primary() {
        Some(window) => {
            for (mut transform, pos) in entities.iter_mut() {
                transform.translation = Vec3::new(
                    convert(pos.x as f32, window.width() as f32, dimensions.width as f32),
                    convert(
                        pos.y as f32,
                        window.height() as f32,
                        dimensions.height as f32,
                    ),
                    0.0,
                )
            }
        }
        None => {
            info!("No window found");
        }
    }
}
