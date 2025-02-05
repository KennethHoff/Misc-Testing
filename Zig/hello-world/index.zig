const std = @import("std");

pub fn main() !void {
    const stdout = std.io.getStdOut().writer();
    const stdin = std.io.getStdIn().reader();

    var buffer = [_]u8{0} ** 32;

    const bytes_read = try stdin.readAll(&buffer);
    const input_str = buffer[0 .. bytes_read - 1];

    try stdout.print("Hello, {s}!\n", .{input_str});
}
