
import os
os.environ["KMP_DUPLICATE_LIB_OK"]="TRUE"
import torch


model = torch.load('last.pt', weights_only= False)

