namespace DEF;

public class SkipList
{
    class SkipNode
    {
        public long Score { get; }
        public string MemberId { get; }
        public SkipNode[] Next { get; }

        public int[] Span; // 跨度记录

        public SkipNode(long score, string member_id, int level)
        {
            Score = score;
            MemberId = member_id;
            Next = new SkipNode[level];
            Span = new int[level];
        }
    }

    int CurrentLevel { get; set; } = 1;
    SkipNode Head { get; set; } = new(long.MinValue, "", MaxLevel);
    Random Rd { get; set; } = new();
    Dictionary<string, long> MemberScores { get; set; } = new();

    const int MaxLevel = 16;

    public void Close()
    {
        Head = null;
        Rd = null;
        MemberScores.Clear();
        MemberScores = null;
    }

    public void Clear()
    {
        CurrentLevel = 1;
        Head = new(long.MinValue, "", MaxLevel);
        MemberScores.Clear();
    }

    public void Upsert(long score, string member_id)
    {
        if (MemberScores.ContainsKey(member_id))
        {
            Delete(MemberScores[member_id], member_id);
        }

        SkipNode[] update = new SkipNode[MaxLevel];
        int[] rank = new int[MaxLevel];
        SkipNode current = Head;

        // 计算各层更新节点和跨度
        for (int i = CurrentLevel - 1; i >= 0; i--)
        {
            rank[i] = i == CurrentLevel - 1 ? 0 : rank[i + 1];
            while (current.Next[i] != null &&
                  (current.Next[i].Score > score ||
                  (current.Next[i].Score == score &&
                   string.Compare(current.Next[i].MemberId, member_id, StringComparison.Ordinal) < 0)))
            {
                rank[i] += current.Span[i];
                current = current.Next[i];
            }
            update[i] = current;
        }

        int new_level = RandomLevel();
        if (new_level > CurrentLevel)
        {
            for (int i = CurrentLevel; i < new_level; i++)
            {
                rank[i] = 0;
                update[i] = Head;
                update[i].Span[i] = MemberScores.Count;
            }
            CurrentLevel = new_level;
        }

        SkipNode new_node = new(score, member_id, new_level);
        for (int i = 0; i < new_level; i++)
        {
            new_node.Next[i] = update[i].Next[i];
            update[i].Next[i] = new_node;
            new_node.Span[i] = update[i].Span[i] - (rank[0] - rank[i]);
            update[i].Span[i] = rank[0] - rank[i] + 1;
        }

        // 更新上层跨度
        for (int i = new_level; i < CurrentLevel; i++)
        {
            update[i].Span[i]++;
        }

        MemberScores[member_id] = score;
    }

    public bool Delete(long score, string member_id)
    {
        SkipNode[] update = new SkipNode[MaxLevel];
        SkipNode current = Head;

        // 查找需要更新的节点
        for (int i = CurrentLevel - 1; i >= 0; i--)
        {
            while (current.Next[i] != null &&
                  (current.Next[i].Score > score ||
                  (current.Next[i].Score == score &&
                   string.Compare(current.Next[i].MemberId, member_id, StringComparison.Ordinal) < 0)))
            {
                current = current.Next[i];
            }
            update[i] = current;
        }

        current = current.Next[0];
        if (current != null && current.Score == score && current.MemberId == member_id)
        {
            // 更新指针和跨度
            for (int i = 0; i < CurrentLevel; i++)
            {
                if (update[i].Next[i] != current)
                {
                    update[i].Span[i]--;

                    continue;
                }

                update[i].Next[i] = current.Next[i];
                update[i].Span[i] += current.Span[i] - 1;
            }

            // 调整当前最大层数
            while (CurrentLevel > 1 && Head.Next[CurrentLevel - 1] == null)
            {
                CurrentLevel--;
            }

            MemberScores.Remove(member_id);

            return true;
        }
        return false;
    }

    public int GetRank(string member_id)
    {
        if (!MemberScores.TryGetValue(member_id, out long score))
        {
            return -1;
        }

        int rank = 0;
        SkipNode current = Head;

        // 通过跨度快速计算排名
        for (int i = CurrentLevel - 1; i >= 0; i--)
        {
            while (current.Next[i] != null &&
                  (current.Next[i].Score > score ||
                  (current.Next[i].Score == score &&
                   string.Compare(current.Next[i].MemberId, member_id, StringComparison.Ordinal) < 0)))
            {
                rank += current.Span[i];
                current = current.Next[i];
            }
        }

        return rank + 1;
    }

    public List<string> GetRange(int start_rank, int end_rank)
    {
        List<string> result = new();

        SkipNode current = Head.Next[0];
        int count = 0;
        while (current != null && count < end_rank)
        {
            if (++count >= start_rank)
            {
                result.Add(current.MemberId);
            }
            current = current.Next[0];
        }

        return result;
    }

    public List<string> GetTopN(int n)
    {
        List<string> result = new();

        SkipNode current = Head.Next[0];
        while (current != null && result.Count < n)
        {
            result.Add(current.MemberId);
            current = current.Next[0];
        }

        return result;
    }

    int RandomLevel()
    {
        int level = 1;
        while (Rd.NextDouble() < 0.5 && level < MaxLevel)
        {
            level++;
        }

        return level;
    }
}