import sqlite3


db = sqlite3.connect('face.db')
cursor = db.cursor()

#delete table if exists
cursor.execute('''DROP TABLE IF EXISTS face''')

cursor.execute('''CREATE TABLE IF NOT EXISTS face (uuid TEXT PRIMARY KEY, name TEXT, path TEXT, embedding BLOB)''')