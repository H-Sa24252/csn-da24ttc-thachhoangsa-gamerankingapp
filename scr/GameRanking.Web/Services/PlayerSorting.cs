using GameRanking.Web.Models;

namespace GameRanking.Web.Services;

public static class PlayerSorting
{
    public static void Bubble(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        for (var end = players.Length - 1; end > 0; end--)
        {
            var swapped = false;
            for (var i = 0; i < end; i++)
            {
                if (compare(players[i], players[i + 1]) <= 0)
                    continue;

                Swap(players, i, i + 1);
                swapped = true;
            }

            // Một lượt không đổi chỗ nghĩa là toàn bộ mảng đã có thứ tự.
            if (!swapped)
                break;
        }
    }

    public static void Selection(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        for (var start = 0; start < players.Length - 1; start++)
        {
            var first = start;
            for (var i = start + 1; i < players.Length; i++)
                if (compare(players[i], players[first]) < 0)
                    first = i;

            if (first != start)
                Swap(players, start, first);
        }
    }

    public static void Insertion(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        for (var i = 1; i < players.Length; i++)
        {
            var current = players[i];
            var position = i;
            // Dịch các phần tử đứng sau current trong đoạn đã sắp xếp.
            while (position > 0 && compare(players[position - 1], current) > 0)
            {
                players[position] = players[position - 1];
                position--;
            }
            players[position] = current;
        }
    }

    public static void Merge(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        var buffer = new PublicPlayer[players.Length];
        MergeRange(players, buffer, 0, players.Length, compare);
    }

    private static void MergeRange(PublicPlayer[] players, PublicPlayer[] buffer, int start, int end,
        Comparison<PublicPlayer> compare)
    {
        if (end - start < 2)
            return;

        var middle = start + (end - start) / 2;
        MergeRange(players, buffer, start, middle, compare);
        MergeRange(players, buffer, middle, end, compare);

        // Trộn hai nửa đã sắp xếp vào vùng đệm dùng chung.
        var left = start;
        var right = middle;
        var index = start;
        while (left < middle && right < end)
            buffer[index++] = compare(players[left], players[right]) <= 0
                ? players[left++] : players[right++];
        while (left < middle)
            buffer[index++] = players[left++];
        while (right < end)
            buffer[index++] = players[right++];

        Array.Copy(buffer, start, players, start, end - start);
    }

    public static void Quick(PublicPlayer[] players, Comparison<PublicPlayer> compare) =>
        QuickRange(players, 0, players.Length - 1, compare);

    private static void QuickRange(PublicPlayer[] players, int start, int end, Comparison<PublicPlayer> compare)
    {
        while (start < end)
        {
            var pivot = players[start + (end - start) / 2];
            var left = start;
            var right = end;
            while (left <= right)
            {
                while (compare(players[left], pivot) < 0)
                    left++;
                while (compare(players[right], pivot) > 0)
                    right--;

                if (left > right)
                    break;

                Swap(players, left++, right--);
            }

            // Đệ quy nhánh nhỏ, lặp nhánh lớn để giới hạn độ sâu ngăn xếp.
            if (right - start < end - left)
            {
                QuickRange(players, start, right, compare);
                start = left;
            }
            else
            {
                QuickRange(players, left, end, compare);
                end = right;
            }
        }
    }

    public static void Heap(PublicPlayer[] players, Comparison<PublicPlayer> compare)
    {
        for (var root = players.Length / 2 - 1; root >= 0; root--)
            SiftDown(players, root, players.Length, compare);

        // Đưa phần tử đứng cuối theo bộ so sánh về cuối đoạn chưa sắp xếp.
        for (var end = players.Length - 1; end > 0; end--)
        {
            Swap(players, 0, end);
            SiftDown(players, 0, end, compare);
        }
    }

    private static void SiftDown(PublicPlayer[] players, int root, int length, Comparison<PublicPlayer> compare)
    {
        while (root < length / 2)
        {
            var child = 2 * root + 1;
            if (child + 1 < length && compare(players[child], players[child + 1]) < 0)
                child++;
            if (compare(players[root], players[child]) >= 0)
                break;

            Swap(players, root, child);
            root = child;
        }
    }

    private static void Swap(PublicPlayer[] players, int first, int second) =>
        (players[first], players[second]) = (players[second], players[first]);
}
