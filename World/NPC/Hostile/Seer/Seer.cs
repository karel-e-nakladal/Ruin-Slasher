using Godot;
using System;

public partial class Seer : Enemy
{
	CollisionShape2D hitbox;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hitbox = GetNode<CollisionShape2D>("HitBox");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_area_2d_body_entered(Node body)
	{
		if(body is Player player){
			if(player.isDashing){
				base.TakeDamage();
				player.AddXP(base.xpReward);
			}else{
				player.TakeDamage();
			}
		}
	}
}
