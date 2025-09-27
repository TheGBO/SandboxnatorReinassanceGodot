from scapy.all import sniff, UDP, IP

def handle_packet(pkt):
    if UDP in pkt and (pkt[UDP].sport == 1077 or pkt[UDP].dport == 1077):
        print(f"UDP {pkt[IP].src}:{pkt[UDP].sport} -> {pkt[IP].dst}:{pkt[UDP].dport}")
        print(bytes(pkt[UDP].payload).hex())

print("Sniffing UDP on loopback interface...")
sniff(iface="lo", prn=handle_packet, store=0)
