
with open("data.json", 'w') as write:

    write.write("{ \"nodes\": [ ")

    with open("nodes.csv", 'r') as nodes:
        head = nodes.readline()
        first = True

        for node in nodes:
            if not first:
                write.write(",")
            first = False
            # print(node)
            node_id, url = node.strip().split(",", 1)
            write.write(" {{ \"id\": {0}, \"label\": {1} }}".format(node_id, url))

    write.write(" ], \"edges\": [ ")

    with open("links.csv", 'r') as links:
        head = links.readline()
        first = True
        l_id = 0

        for link in links:
            if not first:
                write.write(",")
            first = False
            id1, id2 = link.strip().split(",", 1)
            l_id += 1
            write.write(" {{ \"id\": {0}, \"source\": {1}, \"target\": {2} }}".format(l_id, id1, id2))

    write.write(" ] }")
