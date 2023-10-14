namespace Jaco
{
	public interface IMultiArrayKeys<TKey>
	{
		KeyAddress this[TKey key] { get; }//get key address
		int Rank { get; }//count of array dimensions
		int Size { get; }//array size (product of all dimension lengths)
		int GetKeyCount (int dimIdx);//count of keys in dimIdx-dimension
	}
}
