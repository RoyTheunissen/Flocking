using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	/// <summary>
	/// This is a parameter that is randomized.
	/// </summary>
	[System.Serializable]
	public class FlockingParameters
	{
		// General movement.
		public RandomizedParameter acceleration;
		public RandomizedParameterSquared maxSpeed;
		
		// Flock following.
		public RandomizedParameter flockFollowingStrength;

		// Flock velocity matching.
		public RandomizedParameter flockVelocityMatchingStrength;

		// Neighbour avoidance.
		public RandomizedParameterSquared minDistanceToNeighbour;
		public RandomizedParameter neighbourAvoidanceStrength;

		// Landing.
		public RandomizedParameter landCoursePrecision;
		public RandomizedParameterSquared landThreshold;
		public RandomizedParameterSquared slowDownForLandThreshold;
	}
}
