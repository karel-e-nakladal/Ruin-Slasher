using Godot;
using System;

public partial class Enemy : Node2D
{
	
	[Export]
	public int health = 1;
	[Export]
	public float xpReward = 2f;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void TakeDamage(){
		health -= 1;
		if(health <= 0){
			Die();
		}
	}
	private void Die()
	{
		/*Node2D node = GetNode<Node2D>("/root/World");
		TileMapLayer bloodLayer = node.GetNode<TileMapLayer>("Blood");
		TileMapLayer groundLayer = node.GetNode<TileMapLayer>("Ground");
		
		Vector2I tileCoords = groundLayer.LocalToMap(Position);
		
		int sourceId = 0;
		
		TileData baseTile = groundLayer.GeCellTileData(tileCoords);

		GD.Print(baseTile.TextureOrigin);
		bloodLayer.SetCell(tileCoords, 7, new Vector2I(7,7));*/
		QueueFree();
	}
}
