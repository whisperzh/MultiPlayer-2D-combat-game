using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Tilemaps;



public class TileMapMorpher : MonoBehaviour
{
    public Tilemap tilemap;//引用的Tilemap

    public Tile upperTile, lowerTile;

    public GameObject ShatteredGlass;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void des(Vector2 v,Quaternion q,Vector2 flyingdir) {

        Vector3Int cellPos = tilemap.WorldToCell(v);
        Vector2 cellposf = tilemap.CellToWorld(cellPos);//new Vector2(cellPos.x, cellPos.y);
        GameObject f=Instantiate(ShatteredGlass, v+flyingdir.normalized, q);
        if (tilemap.HasTile(cellPos))
        {
            audioSource.PlayOneShot(audioSource.clip);
            tilemap.SetTile(cellPos, null);
        }
        Vector3Int cellposup = cellPos + new Vector3Int(0, 1, 0);
        Vector3Int cellposlow= cellPos - new Vector3Int(0, 1, 0);
        if (tilemap.HasTile(cellposup))
        {
            tilemap.SetTile(cellposup, upperTile);
        }
        if (tilemap.HasTile(cellposlow))
        {
            tilemap.SetTile(cellposlow, lowerTile);
        }
    }



}