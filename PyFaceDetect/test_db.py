import sqlite3
import numpy as np
import pandas as pd
from deepface.commons import distance as dst

db = sqlite3.connect('face.db', check_same_thread=False)
cursor = db.cursor()

df = pd.read_sql_query("SELECT * FROM face", db)
query_embedding = np.random.rand(512)
print(query_embedding.shape)
for embedding in df['embedding']:
    print(np.frombuffer(embedding).shape)
# df['distance'] = df['embedding'].apply(lambda x: dst.findCosineDistance(query_embedding, np.frombuffer(x, dtype=np.float32)))
# print(df)