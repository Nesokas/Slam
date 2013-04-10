function OnMouseEnter()
{
	renderer.material.color = Color.yellow;
}

function OnMouseExit()
{
	renderer.material.color = Color.white;
}

function OnMouseUp()
{
	if (this.tag == "team1") {
		if(this.name == "a") 
			a = 1;
	}
}