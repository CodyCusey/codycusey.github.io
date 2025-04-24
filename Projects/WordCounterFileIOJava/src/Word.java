
public class Word implements Comparable <Word>
{
	private String word;
	private int frequency;

	public Word(String word, int frequency)
	{
		super();
		this.word = word;
		this.frequency = frequency;
	}

	public String getWord()
	{
		return word;
	}

	public void setWord(String word)
	{
		this.word = word;
	}
	
	public int getFrequency()
	{
		return frequency;
	}

	public void setFrequency(int frequency)
	{
		this.frequency = frequency;
	}
	
	@Override
	public int compareTo(Word o)
	{
		return Integer.valueOf(frequency).compareTo(Integer.valueOf(o.frequency));
	}

	@Override
	public String toString()
	{
		return "Word = " + word + ", frequency = " + frequency + "";
	}
}
