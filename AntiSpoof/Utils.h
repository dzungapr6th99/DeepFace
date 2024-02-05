#pragma once
#include <vector>
#include<string>
#include<algorithm>
#include"pch.h"

using namespace std;
namespace AntiSpoofing
{
	class Utils
	{
	public:
		static vector<int64_t> get_kernel(int h_input, int w_input)
		{
			return { (h_input + 15) / 16, (w_input + 15) / 16 };
		}

		static vector<int> get_width_height(string patch_info)
		{
			vector<string> info = split(patch_info, "_");
			for (int i = 0; i < info.size(); i++)
			{
				if (info[i].find('x') > 0)
				{
					vector<string> width_height = split(info[i], "x");
					try
					{
						vector<int> result;
						result.push_back(stoi(width_height[0]));
						result.push_back(stoi(width_height[1]));
						return result;
					}
					catch (exception ex)
					{
						continue;
					}
				}
			}
		}

		static vector<string> parse_model_name(string p_name)
		{
			vector<string> model_info = split(split(p_name, ".pth")[0], "_");
			vector<string> result;
			result.push_back(model_info[model_info.size() - 2]);
			result.push_back(model_info[model_info.size() - 1]);
			return result;
		}

		static vector<string> split(string s, string delimiter)
		{

			vector<string> result;
			int start, end;
			start = end = 0;


			while ((start = s.find_first_not_of(delimiter, end))
				!= string::npos)
			{
				end = s.find(delimiter, start);
				result.push_back(s.substr(start, end - start));
			}
			return result;
		}

		static float Min(float a, float b, float c)
		{
			if (a >= b && a >= c)
				return a;
			else if (b >= a && b >= c)
				return b;
			else
				return c;
		}
	};
}
