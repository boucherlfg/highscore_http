import json
import os
import socket

entry_count : int = 10

def console_async():
    while True:
        line = str(input(" > "))
        if line.startswith("clear"):
            clear_scores()
        elif line.startswith("get"):
            [print(f"{entry.position} - {entry.name} - {entry.score}") for entry in from_json()]
        elif line.startswith("post"):
            name = line.split(' ')[1]
            score = int(line.split(' ')[2])
            highscores = from_json()
            add_score(highscores, name, score)
            highscores = highscores[0:entry_count]
            to_json(highscores)
            [print(f"{entry.position} - {entry.name} - {entry.score}") for entry in highscores]
        elif line.startswith("help"):
            print("clear get post help")

class Entry:
    name : str
    score : int
    position : int

    def __init__(self, name : str = "-", score : int = 0, position : int = -1):
        self.name = name
        self.score = score
        self.position = position
    
    def __str__(self):
        return json.dumps(self.__dict__)

def add_score(highscores : list[Entry], name : str, score : int):
    entry = Entry(name, score, 1000)

    for entry2 in highscores:
        if entry.score > entry2.score and entry.position > entry2.position:
            entry.position = entry2.position
    
    for entry2 in [e for e in highscores if e.position >= entry.position]:
        entry2.position += 1
    
    highscores.append(entry)
    highscores.sort(key = lambda e : e.position)
    print("")


def get_ip() -> str:
    host = socket.gethostname()
    return socket.gethostbyname(host)


def get_value_for(content : str, name : str):
    return [i.split("=")[1] for i in content.split("&") if i.startswith(name)][0]

def clear_scores():
    os.remove("save.txt")

def from_json():
    try:
        with (open("save.txt", "x+") if not os.path.exists("save.txt") else open("save.txt", "r")) as f:
            content = f.read()
            result : list[Entry] = []
            for line in content.split('\n'):
                values = [u for u in line.strip().split(' ')]
                result.append(Entry(values[1], int(values[2]), int(values[0])))
            return result
    except Exception as e:
        print(e)
        return [Entry(position = i) for i in range(0, entry_count)]

# TODO : faire un serializer custom
def to_json(content : list[Entry]):
    with open("save.txt", "w") as f:
        result = "\n".join([f"{u.position} {u.name} {u.score}" for u in content])
        f.write(result)
