using System;
using System.Threading.Tasks;
using ZXing;

namespace WshLst.Core.Interfaces
{
	public interface IBarcodeScanner
	{
		Task<Result> Scan();

		void LookupProduct(string upc, Action<Models.ScanditProduct> onComplete);
	}
}