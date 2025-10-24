from scapy.all import sniff, UDP, IP
import msgpack
import json

def bytesToSpacedHex(data: bytes) -> str:
    return ' '.join(f'{b:02x}' for b in data)

def stripHeader(raw: bytes) -> bytes:
    """
    Strip ENet/Godot header by scanning for the first MessagePack marker.
    Markers: 0x92 = array[2], 0x93 = array[3], 0x83 = map[3], 0x84 = map[4]
    Returns the remainder of the payload starting at the first marker.
    """
    for i, b in enumerate(raw):
        if b in (0x92, 0x93, 0x83, 0x84):
            return raw[i:]
    return raw  # fallback if nothing found

def map_objects(obj):
    """
    Converts array-style ChatMessages into dicts with keys.
    PlayerProfileData maps (dict) are returned as-is.
    """
    # ChatMessage is always an array [Content, PlayerId]
    if isinstance(obj, list) and len(obj) == 2 and isinstance(obj[0], str) and isinstance(obj[1], int):
        return {"Content": obj[0], "PlayerId": obj[1]}
    # PlayerProfileData is already a dict with PlayerName, PlayerColor, PlayerFaceId
    if isinstance(obj, dict):
        return obj
    # Otherwise, return as-is
    return obj

def handlePacket(pkt):
    if UDP in pkt and (pkt[UDP].sport == 1077 or pkt[UDP].dport == 1077):
        raw = bytes(pkt[UDP].payload)
        payload = stripHeader(raw)
        if not payload or len(payload) < 20:  # ignore tiny garbage
            return

        print(f"\n=======[PACKET]=======")
        print(f"UDP {pkt[IP].src}:{pkt[UDP].sport} -> {pkt[IP].dst}:{pkt[UDP].dport}")
        print(f"[hex]: {bytesToSpacedHex(payload)}")
        print(f"[txt]: {payload.decode('utf-8', errors='replace')}")

        # Safe unpacker
        unpacker = msgpack.Unpacker(
            raw=False,
            strict_map_key=False,
            max_array_len=10000,
            max_map_len=10000
        )

        try:
            unpacker.feed(payload)
            for obj in unpacker:
                mapped = map_objects(obj)
                try:
                    print(json.dumps(mapped, indent=2, ensure_ascii=False))
                except Exception:
                    print("[Warning] Could not convert object to JSON:", mapped)
        except Exception as e:
            print("[Warning] Could not unpack payload:", e)

print("Sniffing UDP on loopback interface...")
sniff(iface="lo", prn=handlePacket, store=0)
