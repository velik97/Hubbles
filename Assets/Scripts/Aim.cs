
/// <summary>
/// Aim for level. Deprecated
/// </summary>
[System.Serializable]
public class Aim {

	public int color;
	public int type;
	public int count;

	public Aim () {
		this.color = 0;
		this.type = 0;
		this.color = 0;
	}

	public Aim (int color, int type, int count) {
		this.color = color;
		this.type = type;
		this.count = count;
	}

	public Aim Copy () {
		return new Aim (color, type, count);
	}

	public bool isDone () {
		return count <= 0;
	}
		
}
