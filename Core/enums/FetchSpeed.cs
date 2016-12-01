namespace enums
{
	public class FetchSpeed {
		
		public FetchSpeedEnum Speed;

		public FetchSpeed(FetchSpeedEnum speed)
		{
			Speed = speed;
		}
	}

	public enum FetchSpeedEnum
	{
		FAST,
		SLOW,
		MEDIUM
	}
}

