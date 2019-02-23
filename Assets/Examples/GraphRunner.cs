using PiRhoSoft.CompositionEngine;
using UnityEngine;

public class GraphRunner : MonoBehaviour, IVariableStore
{
	public InstructionCaller GraphCaller;

	void Start()
	{
		CompositionManager.Instance.RunInstruction(GraphCaller, null, this);
	}

	public VariableValue GetVariable(string name)
	{
		return VariableValue.Empty;
	}

	public SetVariableResult SetVariable(string name, VariableValue value)
	{
		return SetVariableResult.NotFound;
	}
}
