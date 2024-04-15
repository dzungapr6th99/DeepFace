#include "Distance.h"
namespace MLCpp
{
	float Distance::findEuclideanDistance(float source_Presentation[], float test_Presentation[])
	{
		at::Tensor a = at::from_blob(source_Presentation, { 1,2 });
		at::Tensor b = at::from_blob(test_Presentation, { 1,2 });
		at::Tensor Euclide = a - b;
	    
		return Euclide.pow(2).sum().sqrt().item<float>();

			
	}
	float Distance::findCosineDistance(float source_Presentation[], float test_Presentation[])
	{
		at::Tensor source = at::from_blob(source_Presentation, { 1,2 });
		at::Tensor test = at::from_blob(test_Presentation, { 1,2 });
		at::Tensor a = at::matmul(at::transpose(source,1,2), test);
		at::Tensor b = at::sum(at::mul(source, source));
		at::Tensor c = at::sum(at::mul(test, test));
		at::Tensor nResult = (a / (at::sqrt(b) * at::sqrt(c)));
		return 1- nResult.item<float>();
		
	}
}