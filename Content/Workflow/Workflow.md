The ideal order for a piece of work on a branch, from start to merge. In practice, steps loop back or come out of order. For example, after a big fix, review again, and if the tidy finds bugs, go back to bug fixing. Skip steps that don't fit the change, such as a behaviour checklist for server setup, and note which ones in the progress doc.

### When your part is done

Add it to the branch's progress [temporary doc](Temporary%20Docs.md), starting one if there isn't one. Then tell the user which step comes next, and which role takes it. Do this when a piece of work is finished, not after each small change while the user is iterating.

### The steps

1. [Feature Implementor](../Roles/Feature%20Implementor.md): write the change.
2. User: initial manual testing, to confirm the behaviour.
3. [Behaviour Documenter](../Roles/Behaviour%20Documenter.md): add or update the behaviour checklist.
4. [Check Writer](../Roles/Check%20Writer.md): checks for what can be checked automatically, and the start of the manual tests for the rest.
5. [Code Reviewer](../Roles/Code%20Reviewer.md): review the branch.
6. [Bug Fixer](../Roles/Bug%20Fixer.md): work through the bug list.
7. [Code Tidier](../Roles/Code%20Tidier.md): tidy, and pass findings that change behaviour to the bug list.
8. User: code review. The [Review Responder](../Roles/Review%20Responder.md) makes the changes asked for, and proposes general rules from them.
9. [Tester](../Roles/Tester.md): run what an agent can. The rest goes in the manual tests for the user.
10. User: manual testing.

### Ready to merge when

- Every bug is fixed, or left with a reason the user has agreed to.
- The user has tried it, and every manual test is ticked off, passed by the Tester and accepted by the user, or skipped with a reason they agreed to.
- The Tester has walked through the behaviour checklist against the automated checks and manual tests, and every item is covered.
- The branch's temporary docs have been deleted.
