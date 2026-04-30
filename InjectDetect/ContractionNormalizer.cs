using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class ContractionNormalizer
    {
        // Expand: contraction -> full form
        private static readonly Dictionary<string, string> ExpandMap = new(System.StringComparer.OrdinalIgnoreCase)
        {
            // negations
            { "don't",      "do not"        },
            { "dont",       "do not"        },
            { "doesn't",    "does not"      },
            { "doesnt",     "does not"      },
            { "didn't",     "did not"       },
            { "didnt",      "did not"       },
            { "won't",      "will not"      },
            { "wont",       "will not"      },
            { "wouldn't",   "would not"     },
            { "wouldnt",    "would not"     },
            { "shouldn't",  "should not"    },
            { "shouldnt",   "should not"    },
            { "couldn't",   "could not"     },
            { "couldnt",    "could not"     },
            { "can't",      "cannot"        },
            { "cant",       "cannot"        },
            { "isn't",      "is not"        },
            { "isnt",       "is not"        },
            { "aren't",     "are not"       },
            { "arent",      "are not"       },
            { "wasn't",     "was not"       },
            { "wasnt",      "was not"       },
            { "weren't",    "were not"      },
            { "werent",     "were not"      },
            { "hasn't",     "has not"       },
            { "hasnt",      "has not"       },
            { "haven't",    "have not"      },
            { "havent",     "have not"      },
            { "hadn't",     "had not"       },
            { "hadnt",      "had not"       },
            { "mustn't",    "must not"      },
            { "mustnt",     "must not"      },
            { "needn't",    "need not"      },
            { "neednt",     "need not"      },
            { "daren't",    "dare not"      },
            { "darent",     "dare not"      },

            // be / have / will
            { "i'm",        "i am"          },
            { "im",         "i am"          },
            { "you're",     "you are"       },
            { "youre",      "you are"       },
            { "he's",       "he is"         },
            { "hes",        "he is"         },
            { "she's",      "she is"        },
            { "shes",       "she is"        },
            { "it's",       "it is"         },
            { "its",        "it is"         },
            { "we're",      "we are"        },
            { "were",       "we are"        },
            { "they're",    "they are"      },
            { "theyre",     "they are"      },
            { "i've",       "i have"        },
            { "ive",        "i have"        },
            { "you've",     "you have"      },
            { "youve",      "you have"      },
            { "we've",      "we have"       },
            { "weve",       "we have"       },
            { "they've",    "they have"     },
            { "theyve",     "they have"     },
            { "i'll",       "i will"        },
            { "ill",        "i will"        },
            { "you'll",     "you will"      },
            { "youll",      "you will"      },
            { "he'll",      "he will"       },
            { "hell",       "he will"       },
            { "she'll",     "she will"      },
            { "shell",      "she will"      },
            { "we'll",      "we will"       },
            { "well",       "we will"       },
            { "they'll",    "they will"     },
            { "theyll",     "they will"     },
            { "i'd",        "i would"       },
            { "id",         "i would"       },
            { "you'd",      "you would"     },
            { "youd",       "you would"     },
            { "he'd",       "he would"      },
            { "hed",        "he would"      },
            { "she'd",      "she would"     },
            { "shed",       "she would"     },
            { "we'd",       "we would"      },
            { "wed",        "we would"      },
            { "they'd",     "they would"    },
            { "theyd",      "they would"    },

            // misc
            { "let's",      "let us"        },
            { "lets",       "let us"        },
            { "that's",     "that is"       },
            { "thats",      "that is"       },
            { "there's",    "there is"      },
            { "theres",     "there is"      },
            { "here's",     "here is"       },
            { "heres",      "here is"       },
            { "what's",     "what is"       },
            { "whats",      "what is"       },
            { "who's",      "who is"        },
            { "whos",       "who is"        },
            { "how's",      "how is"        },
            { "hows",       "how is"        },
            { "where's",    "where is"      },
            { "wheres",     "where is"      },
            { "when's",     "when is"       },
            { "whens",      "when is"       },
            { "why's",      "why is"        },
            { "whys",       "why is"        },
            { "could've",   "could have"    },
            { "couldve",    "could have"    },
            { "should've",  "should have"   },
            { "shouldve",   "should have"   },
            { "would've",   "would have"    },
            { "wouldve",    "would have"    },
            { "might've",   "might have"    },
            { "mightve",    "might have"    },
            { "must've",    "must have"     },
            { "mustve",     "must have"     },
            { "ain't",      "am not"        },
            { "aint",       "am not"        },
        };

        // Contract: full form -> contraction (reverse map, longest phrase first)
        private static readonly List<(string Full, string Contracted)> ContractList;

        static ContractionNormalizer()
        {
            // Build contract list from expand map, deduplicated, sorted longest first
            var seen = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            ContractList = new List<(string, string)>();
            foreach (var kv in ExpandMap)
            {
                if (seen.Add(kv.Value))
                    ContractList.Add((kv.Value, kv.Key));
            }
            ContractList.Sort((a, b) => b.Full.Length.CompareTo(a.Full.Length));
        }

        public static string Expand(string input)
        {
            return Regex.Replace(input, @"[\w']+", m =>
                ExpandMap.TryGetValue(m.Value, out string? expanded) ? expanded : m.Value);
        }

        public static string Contract(string input)
        {
            string result = input;
            foreach (var (full, contracted) in ContractList)
            {
                result = Regex.Replace(
                    result,
                    @"\b" + Regex.Escape(full) + @"\b",
                    contracted,
                    RegexOptions.IgnoreCase
                );
            }
            return result;
        }
    }
}