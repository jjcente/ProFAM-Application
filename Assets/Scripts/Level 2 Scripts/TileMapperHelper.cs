using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHelper : MonoBehaviour
{
    public Tilemap floorTilemap; // assign floor tilemap
    public BoundsInt GetBounds() => floorTilemap.cellBounds;

    public Vector3Int GetRandomWalkableCell()
    {
        BoundsInt b = floorTilemap.cellBounds;
        Vector3Int cell;
        int tries = 0;
        do
        {
            cell = new Vector3Int(Random.Range(b.xMin, b.xMax), Random.Range(b.yMin, b.yMax), 0);
            tries++;
            if (tries > 1000) break;
        } while (!floorTilemap.HasTile(cell));
        return cell;
    }

    public Vector3 cellToWorldCenter(Vector3Int cell) => floorTilemap.GetCellCenterWorld(cell);
}
