#pragma strict

var acceleration = 0.2;
var shootVelocity = 11.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

var team = 1;
var player_name = "Ines";
var player_num = 1;

private var direction = Vector3.zero;
private var ball_collision = false;
private var shoot_material : Material;
private var normal_material : Material;
var gamepad_num : int = 0;
private var colliding_with_ball = false;
private var ball_collider : Collision;
private var base : Transform;
private var ball : GameObject;


function InitializePlayerInfo(num:int, team_num:int, player_number:int)
{
	gamepad_num = num;
	gamepad = true;
	team = team_num;
	player_num = player_number;
	
	Start();
}

function VerifyShoot()
{
	if(colliding_with_ball && !ball_collision){
		if((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))){
			ball_collider.rigidbody.velocity -= ball_collider.contacts[0].normal * shootVelocity;
			ball_collision = true;
			colliding_with_ball = false;
		}
	}
}

function OnCollisionEnter(collision : Collision)
{	
	if(collision.gameObject.name == "Ball" || collision.collider.tag == "colliderShoot") {
		colliding_with_ball = true;
		ball_collider = collision;
	}
}

function OnCollisionExit(collision : Collision)
{
	if(collision.gameObject.name == "Ball" || collision.collider.tag == "colliderShoot") {
		colliding_with_ball = false;
	}
}

function increase_speed() 
{
	if(gamepad){
		direction.x = Input.GetAxis("Vertical_Gamepad_" + gamepad_num);
		direction.z = Input.GetAxis("Horizontal_Gamepad_" + gamepad_num);
	} else {
		direction.x = Input.GetAxis("Vertical");
		direction.z = Input.GetAxis("Horizontal");
	}
	direction.Normalize();
		
	rigidbody.velocity += direction*acceleration;
}

function updateRotation()
{
	if(!ball)
		ball = GameObject.FindGameObjectWithTag("ball");
	var rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
}

function Start() {
	
	var court_walls = GameObject.FindGameObjectWithTag("court_walls");
	var goal_detection = GameObject.FindGameObjectsWithTag("goal_detection");
	var posts = GameObject.FindGameObjectsWithTag("post");
	var players = GameObject.FindGameObjectsWithTag("Player");
	base = transform.Find("Base");
	var base_collider = base.Find("Collider");
	var shoot_collider = base.Find("ColliderShoot");
	Physics.IgnoreCollision(court_walls.collider, base_collider.collider);
	Physics.IgnoreCollision(court_walls.collider, shoot_collider.collider);
	for(var i = 0; i < goal_detection.Length; i++) {
		Physics.IgnoreCollision(goal_detection[i].collider, base_collider.collider);
		Physics.IgnoreCollision(goal_detection[i].collider, shoot_collider.collider);
	}
	for(i = 0; i < posts.Length; i++) {
		Physics.IgnoreCollision(posts[i].collider, shoot_collider.collider);
	}
	Debug.Log(transform.gameObject);
	
	for (i=0; i < players.Length; i++) {
		var player_base = players[i].transform.Find("Base");
		var player_shoot_collider = player_base.transform.Find("ColliderShoot");
		if(player_shoot_collider.collider != shoot_collider.collider) {
			Physics.IgnoreCollision(player_shoot_collider.collider, shoot_collider.collider);
			Physics.IgnoreCollision(player_shoot_collider.collider, base_collider.collider);
		}
	}
	
	if(team == 1) {
		normal_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player1.mat", typeof(Material)) as Material;
		shoot_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player1_shoot.mat", typeof(Material)) as Material;
	} else {
		normal_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player2.mat", typeof(Material)) as Material;
		shoot_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player2_shoot.mat", typeof(Material)) as Material;
	}

	base.renderer.material = normal_material;
}

function Update ()
{
	if(base == null)
		base = transform.Find("Base");
		
	if(ball_collision && !((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num))))) {
		ball_collision = false;
	}
	
	if(!ball_collision && ((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))))
		base.renderer.material = shoot_material;
	else
		base.renderer.material = normal_material;
	
	increase_speed();

	VerifyShoot();
	
//	if(team == 1 && player_num == 1)
	//	Debug.Log(rigidbody.velocity);
		
	updateRotation();
	
}