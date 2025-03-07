using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int speed {get; set;} = 100;
	[Export]
	public float dashTime {get; set;} = .05f;
	[Export]
	public int dashRange {get; set;} = 100;
	[Export]
	public float animationDeadZone {get; set;} = 0.5f;
	[Export]
	public int MaxHealth {get; set;} = 4;
	[Export]
	public float ShieldDuration {get; set;} = 0.35f;
	[Export]
	public int DashMaxCount {get; set;} = 3;
	[Export]
	public float DashCooldown {get; set;} = 3.0f;
	[Export]
	public int ParryMaxCount {get; set;} = 1;
	[Export]
	public float ParryCooldown {get; set;} = 1.0f;
	[Export]
	public int XpOffset{get; set;} = 1;

	public bool isDashing {get; set;} = false;
	public bool isShielded {get; set;} = false;
	public int SelectedAbility = 1;

	private int Health;
	private float dashTimeLeft = 0f;
	private float dashCooldownLeft = 0f;
	private float ShieldTimeLeft = 0f;
	private int DashCount;
	private int ParryCount;
	private float xp = 0;
	private int lvl = 1;

	AudioStreamPlayer attackSound;
	AudioStreamPlayer BlockSound;
	AudioStreamPlayer lvlUpSound;
	AnimationPlayer CAP;
	AnimationPlayer EAP;
	Sprite2D sprite;
	Sprite2D healthBar;
	Sprite2D dashBar;
	Sprite2D xpBar;
	Timer bulletTimeTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CAP = GetNode<AnimationPlayer>("Animations/CharacterAnimationPlayer");
		EAP = GetNode<AnimationPlayer>("Animations/EffectsAnimationPlayer");		
		sprite = GetNode<Sprite2D>("PlayerSprite");
		attackSound = GetNode<AudioStreamPlayer>("Audio/AttackSound");
		BlockSound = GetNode<AudioStreamPlayer>("Audio/BlockSound");
		lvlUpSound = GetNode<AudioStreamPlayer>("Audio/LvlUpSound");
		Health = MaxHealth;
		DashCount = DashMaxCount;
		ParryCount = ParryMaxCount;
		healthBar = GetNode<Sprite2D>("Control/HealthBar");
		dashBar = GetNode<Sprite2D>("Control/DashBar");
		xpBar = GetNode<Sprite2D>("Control/XPBar");
		bulletTimeTimer = GetNode<Timer>("BulletTimeTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(isDashing){
			dashTimeLeft -= (float) delta;
			if(dashTimeLeft <= 0){
				isDashing = !isDashing;
			}
		}else{
			Move(delta);
		}
		if(DashCount < DashMaxCount)
		{
			dashCooldownLeft -= (float) delta;
			if(dashCooldownLeft <= 0 )
			{
				
			}
		}
		if(isShielded){
			ShieldTimeLeft -= (float) delta;
			if(ShieldTimeLeft <= 0){
				isShielded = !isShielded;
			}
		}
		
		MoveAndSlide();
	}
	public void _on_bullet_time_timer_timeout()
	{
		Engine.TimeScale = 1;
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("move_attack")){
			if(!isDashing && DashCount > 0){
				isDashing = true;
				dashTimeLeft = dashTime;
				attackSound.Play();
				DashCount--;
				dashBar.Frame = dashBar.Frame + 1;
				Dash();
			}
		}
		if(@event.IsActionPressed("move_ability")){
			CastAbility();
		}
	}
	
	public void AddXP(float xpReward)
	{	
		Engine.TimeScale = 0.3;
		bulletTimeTimer.Start();
		xp += xpReward;
		int frame = (int)(( (xp / ((float)lvl*XpOffset) / 9)*100))-1;
		if(xp >= lvl*XpOffset){
			frame = 0;
			LvlUp();
		}
		xpBar.Frame = frame;
		GD.Print(frame);
	}
	private void LvlUp()
	{
		lvl++;
		xp = 0;
		lvlUpSound.Play();
	}
	public void TakeDamage()
	{
		if(ShieldTimeLeft > 0){
			BlockSound.Play();
			if(DashCount < DashMaxCount){
				DashCount++;
				dashBar.Frame--;
			}
		}else{
			Health -= 1;
			healthBar.Frame = MaxHealth - Health;
			if(Health <= 0){
				Die();
				Health = 0;
			}
		}
	}
	public void Die()
	{
		GetTree().Quit();
	}
	public void Heal()
	{
		Health += 1;
		if(Health >= MaxHealth)
		{
			Health = MaxHealth;
		}
		healthBar.Frame = MaxHealth - Health;
	}
	private void CastAbility()
	{
		switch(SelectedAbility){
			
			case 0:
				EAP.Play("RESET");
				break;
			case 1:
				EAP.Play("ability_shield");
				isShielded = true;
				ShieldTimeLeft = ShieldDuration;
				break;
		}
	}
	private void Move(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		WalkAnimation(direction);
		if(direction != Vector2.Zero)
		{
			Velocity = Velocity.Lerp(direction.Normalized() * speed, 10  * (float)delta);
		}else
		{
			Velocity = Velocity.Lerp(Vector2.Zero, 20  * (float)delta);
		}
		//Velocity = new Vector2(direction.X * speed * 100  * (float)delta, direction.Y * speed * 100 * (float)delta);
	}
	
	private void Dash()
	{
			//Vector2 direction = Position.DirectionTo(GetGlobalMousePosition());
			Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			DashAnimation(direction);
			Velocity = direction * (dashRange / dashTime);
	}
	
	private void DashAnimation(Vector2 direction)
	{
		if(direction.X > animationDeadZone)
		{
			CAP.Play("dash_right");
		}else if (direction.X < -animationDeadZone)
		{
			CAP.Play("dash_left");
		}
		if(direction.Y < -animationDeadZone)
		{
			CAP.Play("dash_up");
		}else if(direction.Y > animationDeadZone)
		{
			CAP.Play("dash_down");
		}
	}
	private void WalkAnimation(Vector2 direction)
	{
		if(direction.Y < -animationDeadZone)
		{
			CAP.Play("walk_up");
		}else if(direction.Y > animationDeadZone)
		{
			CAP.Play("walk_down");
		}
		if(direction.X > animationDeadZone)
		{
			CAP.Play("walk_right");
		}else if (direction.X < -animationDeadZone)
		{
			CAP.Play("walk_left");
		}
		if(direction == Vector2.Zero){
			CAP.Play("RESET");
		}
	}
	
	
}
