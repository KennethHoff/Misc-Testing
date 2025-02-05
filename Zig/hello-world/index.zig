const std = @import("std");
const bufferSize = 32;

pub fn main() !void {
    const stdout = std.io.getStdOut().writer();
    const stdin = std.io.getStdIn().reader();

    var buffer = [_]u8{0} ** (bufferSize + 1);

    const bytes_read = try stdin.readAll(&buffer);
    if (bytes_read > bufferSize) {
        try stdout.print("Too long, bro. The maxiumum is {d}\n", .{bufferSize});
        return;
    }
    const input_str = buffer[0 .. bytes_read - 1];

    try stdout.print("Hello, '{s}'!\n", .{input_str});
}
