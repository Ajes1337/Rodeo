
using System;
using System.Text;

public class Diverse {


}

public enum PacketType {
    NeedChunkPoses,
    GenMap,
    GenMesh
}

public enum Speed {
    Normal,
    Fast,
    Faster
}

public enum Direction {
    Top, Bottom,
    North, South, East, West,
    NE, SE, SW, NW
}

public struct Vector2I : IEquatable<Vector2I> {

    public int x;
    public int y;

    public Vector2I(int x, int y) {
        this.x = x;
        this.y = y;
    }


    public bool Equals(Vector2I other) {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        return (x + " - " + y).GetHashCode();
    }
}
