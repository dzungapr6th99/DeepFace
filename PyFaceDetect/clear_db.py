import sqlite3

#clear all db
conn = sqlite3.connect('face.db', check_same_thread=False)   
c = conn.cursor()
c.execute("DELETE FROM face")