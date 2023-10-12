
import sys
arg1 = sys.argv[1]
print(arg1);


import stanza

# Download the Hebrew model
stanza.download('he', package='htb')


# Load the Hebrew model
nlp = stanza.Pipeline('he', package='htb')


def get_pure_noun(phrase):
    # Process the phrase with the Hebrew model
    doc = nlp(phrase)

    # Find the first noun in the phrase
    for sent in doc.sentences:
        for word in sent.words:
            if word.upos == 'NOUN' or word.upos == 'ADJ':
                return word.lemma

    # If no noun is found, return None
    return None


# phrase = arg1
# noun = get_pure_noun(phrase)
# print(get_pure_noun("בגרון"))
#print(noun)  # Output: בגד
#print("הצוואר")
