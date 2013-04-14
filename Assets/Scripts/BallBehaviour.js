#pragma strict

function Start () {
	var players = GameObject.FindGameObjectsWithTag("Player");
	
	for(var i = 0; i < players.Length; i++) {
		var player_base = players[i].transform.Find("Base");
		var player_shoot_colider = player_base.transform.Find("ColliderShoot");
		Physics.IgnoreCollision(player_shoot_colider.collider, transform.collider);
	}
}

function Update () {

}